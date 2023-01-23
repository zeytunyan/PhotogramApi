using Api.Configs;
using Api.Exceptions;
using Api.Exceptions.ExistsExceptions;
using Api.Exceptions.IncorrectExceptions;
using Api.Exceptions.NotActiveExceptions;
using Api.Exceptions.NotFoundExceptions;
using Api.Models.Post;
using AutoMapper;
using Common;
using Common.Consts;
using Common.Extensions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Api.Services
{
    public class CheckService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CheckService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task ThrowExIfUserExistsAsync(string email, string phoneNumber, string nickName)
        {
            // Не отсеиваем удаленных, потому что alternate key всё равно не даст добавить
            var exists = await _context.Users.AnyAsync(u =>
                u.Email.ToLower() == email.ToLower()
                || u.PhoneNumber.ToLower() == phoneNumber.ToLower()
                || u.NickName == nickName);

            if (exists) throw new UserExistsException();
        }

        public async Task<User> FindUserOrThrowExAsync(Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            return user ?? throw new UserNotFoundException();
        }

        public async Task<User> FindUserByCredentialsOrThrowExAsync(string login, string pass)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());

            if (user == null)
                throw new UserNotFoundException();

            if (!HashHelper.Verify(pass, user.PasswordHash))
                throw new PasswordIncorrectException();

            return user;
        }


        public async Task<Post> FindPostOrThrowExAsync(Guid postId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

            return post ?? throw new PostNotFoundException();
        }

        public async Task<Comment> FindCommentOrThrowExAsync(Guid commentId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

            return comment ?? throw new CommentNotFoundException();
        }

        public async Task<PostContent> FindPostContentOrThrowExAsync(Guid postContentId)
        {
            var postContent = await _context.PostContents
                 .FirstOrDefaultAsync(pc => pc.Id == postContentId && !pc.IsDeleted);

            return postContent ?? throw new PostContentNotFoundException();
        }

        public async Task<UserSession> FindSessionOrThrowExAsync(Guid sessionId)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            return session ?? throw new SessionNotFoundException();
        }

        public async Task<UserSession> FindSessionByRefreshTokenOrThrowExAsync(Guid refreshTokenId)
        {
            var session = await _context.UserSessions
                .Include(x => x.User) //// await _context.Entry(session).Reference(s => s.User).LoadAsync()?
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshTokenId);

            return session ?? throw new SessionNotFoundException();
        }

        public Guid GetGuidFromClaimsOrThrowEx(ClaimsPrincipal claims, string claimName)
        {
            if (!claims.TryFindGuidValue(claimName, out var guid))
                throw new UnauthorizedException();

            return guid;
        }

        public FileInfo GetFileInfoOrThrowExIfNotExist(string fileName)
        {
            var fileInfo = new FileInfo(fileName);

            if (!fileInfo.Exists)
                throw new Exceptions.NotFoundExceptions.FileNotFoundException();

            return fileInfo;
        } 

        public void ThrowExIfFileExist(string fileName)
        {
            var fileinfo = new FileInfo(fileName);
            if (fileinfo.Exists) throw new FileExistsException();
        }           

        public void ThrowExIfSessionIsNotActive(UserSession session)
        {
            if (!session.IsActive)
                throw new SessionNotActiveException();
        }

        public void ThrowExIfCommentIsNotExist(Comment? comment)
        {
            if (comment == null)
                throw new CommentNotFoundException();
        }

        public void ThrowExIfPostIsNotExist(Post? post)
        {
            if (post == null)
                throw new PostNotFoundException();
        }

        public void ThrowExIfPostContentIsEmpty(InPostModel post)
        {
            if (post.Contents.Count < 1)
                throw new ArgumentException("The post must contain content.");
        }

        public void ThrowExIfGoogleNotFound(PushConfig.GoogleConfig? google)
        {
            if (google == null)
                throw new ArgumentNullException("Google configuration not found");

        }

    }
}
