using Api.Configs;
using Api.Models.Token;
using AutoMapper;
using Common.Consts;
using Common;
using DAL.Entities;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Exceptions;
using Api.Exceptions.NotActiveExceptions;

namespace Api.Services
{
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly AuthConfig _config;
        private readonly CheckService _checkService;

        public AuthService(CheckService checkService, IOptions<AuthConfig> config, DataContext context)
        {
            _context = context;
            _config = config.Value;
            _checkService = checkService;
        }

        public async Task<TokenModel> GetTokenAsync(string login, string password)
        {
            // var user = await GetUserByCredentionAsync(login, password);
            var user = await _checkService.FindUserByCredentialsOrThrowExAsync(login, password);

            var session = await _context.UserSessions.AddAsync(new UserSession
            {
                Id = Guid.NewGuid(),
                User = user,
                RefreshToken = Guid.NewGuid(),
                Created = DateTime.UtcNow
            });
            
            await _context.SaveChangesAsync();
            
            return GenerateTokens(session.Entity);
        }

        public async Task<TokenModel> GetTokenByRefreshTokenAsync(string refreshToken)
        {
            var validParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = _config.SymmetricSecurityKey()
                //ClockSkew = TimeSpan.Zero,
            };
            var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

            //// TryFindGuidValue?
            if (securityToken is not JwtSecurityToken jwtToken
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
                || principal.FindFirstValue("refreshToken") is not string refreshIdString
                || !Guid.TryParse(refreshIdString, out var refreshId))
            {
                throw new SecurityTokenException("Invalid token");
            }

            //var session = await GetSessionByRefreshTokenAsync(refreshId);
            var session = await _checkService.FindSessionByRefreshTokenOrThrowExAsync(refreshId);
            
            _checkService.ThrowExIfSessionIsNotActive(session);
            session.RefreshToken = Guid.NewGuid();
            await _context.SaveChangesAsync();
            return GenerateTokens(session);
        }

        private TokenModel GenerateTokens(UserSession session)
        {
            if (session.User == null)
                throw new ItemException(ItemNames.Session);

            var dtNow = DateTime.Now;

            var jwt = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim(CustomClaimNames.Id, session.User.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, session.User.NickName),
                    new Claim(CustomClaimNames.SessionId, session.Id.ToString()),
                },
                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refresh = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim(CustomClaimNames.RefreshToken, session.RefreshToken.ToString()),
                },
                expires: DateTime.Now.AddHours(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

            return new TokenModel(encodedJwt, encodedRefresh);
        }

        //public async Task<UserSession> GetSessionByIdAsync(Guid sessionId) =>
        //    await _checkService.FindSessionOrThrowExAsync(sessionId);


        //private async Task<User> GetUserByCredentionAsync(string login, string pass) =>
        //    await _checkService.FindUserByCredentialsOrThrowExAsync(login, pass);


        //private async Task<UserSession> GetSessionByRefreshTokenAsync(Guid refreshTokenId) =>
        //    await _checkService.FindSessionByRefreshTokenOrThrowExAsync(refreshTokenId);

    }
}
