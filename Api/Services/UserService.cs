using Api.Models.Attach;
using Api.Models.User;
using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly CheckService _checkService;

        public UserService(IMapper mapper, DataContext context, CheckService checkService)
        {
            _mapper = mapper;
            _context = context;
            _checkService = checkService;
        }

        public async Task<List<UserShortInfoModel>> GetUsersAsync()
        {
            return await _context.Users.AsNoTracking()
            .Include(u => u.Avatar)
            .Include(u => u.Posts)
            .Include(u => u.Followers)
            .Include(u => u.Followings)
            .AsNoTracking()
            .Select(u => _mapper.Map<UserShortInfoModel>(u))
            .ToListAsync();
        }

        public async Task<UserPageInfoModel> CreateUserAsync(UserRegistrationModel model)
        {
            var dbUser = _mapper.Map<User>(model);
            var added = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserPageInfoModel>(added.Entity);
        }

        public async Task<UserShortInfoModel> DeleteUserAsync(Guid id)
        {
            var dbUser = await _checkService.FindUserOrThrowExAsync(id);
            dbUser.IsDeleted = true;
            await _context.SaveChangesAsync();
            return _mapper.Map<UserShortInfoModel>(dbUser);
        }

        public async Task<Guid> AddUserAvatarAsync(Guid userId, InFileMetaModel meta, string filePath)
        {
            await _checkService.FindUserOrThrowExAsync(userId); // var user = -||-
            //// Добавить проверку на существование файла?

            var avatar = _mapper.Map<Avatar>(meta);
            (avatar.AuthorId, avatar.OwnerId) = (userId, userId);
            avatar.FilePath = filePath;

            await _context.Avatars.AddAsync(avatar); // user.Avatar = avatar;
            await _context.SaveChangesAsync();
            return avatar.OwnerId;
        }

        public async Task<AttachModel> GetUserAvatarAsync(Guid userId)
        {
            var user = await _checkService.FindUserOrThrowExAsync(userId);
            await _context.Entry(user).Reference(u => u.Avatar).LoadAsync();
            return _mapper.Map<AttachModel>(user.Avatar);
        }

        public async Task<UserPageInfoModel> GetUserPageInfoAsync(Guid id)
        {
            var user = await _checkService.FindUserOrThrowExAsync(id);

            await _context.Entry(user).Reference(u => u.Avatar).LoadAsync();
            await _context.Entry(user).Collection(u => u.Posts).LoadAsync();
            await _context.Entry(user).Collection(u => u.Followers).LoadAsync();
            await _context.Entry(user).Collection(u => u.Followings).LoadAsync();

            return _mapper.Map<UserPageInfoModel>(user);
        }

        public async Task<UserFullInfoModel> GetUserFullInfoAsync(Guid id)
        {
            var user = await _checkService.FindUserOrThrowExAsync(id);

            await _context.Entry(user).Reference(u => u.Avatar).LoadAsync();

            return _mapper.Map<UserFullInfoModel>(user);
        }


        public async Task<List<UserShortInfoModel>> GetFollowingsAsync(Guid userId)
        {
            await _checkService.FindUserOrThrowExAsync(userId);

            //// Надо ли выдавать удаленных?
            return await _context.Followings
                .Include(f => f.FollowedTo).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(f => 
                    f.FollowerId == userId 
                    && f.UnfollowDate == null 
                    && f.FollowedTo != null 
                    && !f.FollowedTo.IsDeleted)
                .Select(f => _mapper.Map<UserShortInfoModel>(f.FollowedTo))
                .ToListAsync();
        }

        public async Task<List<UserShortInfoModel>> GetFollowersAsync(Guid userId)
        {
            await _checkService.FindUserOrThrowExAsync(userId);

            //// Надо ли выдавать удаленных?
            return await _context.Followings
                .Include(f => f.Follower).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(f => 
                    f.FollowedToId == userId 
                    && f.UnfollowDate == null 
                    && f.Follower != null
                    && !f.Follower.IsDeleted)
                .Select(f => _mapper.Map<UserShortInfoModel>(f.Follower))
                .ToListAsync();
        }

        public async Task<bool> CheckFollowingAsync(Guid follower, Guid followedTo)
        {
            var following = await GetFollowingAsync(follower, followedTo);
            return following != null;
        }

        public async Task<bool> ChangeFollowingAsync(Guid follower, Guid followedTo)
        {
            var following = await GetFollowingAsync(follower, followedTo);
            
            if (following != null)
                following.UnfollowDate = DateTime.UtcNow;

            else
                following = await FollowAsync(follower, followedTo);

            await _context.SaveChangesAsync();
            return following.UnfollowDate == null;
        }

        private async Task<Following> FollowAsync(Guid follower, Guid followedTo)
        {
            var followingRes = await _context.Followings.AddAsync(new()
            {
                FollowerId = follower,
                FollowedToId = followedTo,
                FollowDate = DateTime.UtcNow
            });

            return followingRes.Entity;
        }

        private async Task<Following?> GetFollowingAsync(Guid follower, Guid followedTo)
        {
            await _checkService.FindUserOrThrowExAsync(follower);
            await _checkService.FindUserOrThrowExAsync(followedTo);

            return await _context.Followings
                .OrderByDescending(f => f.FollowDate)
                .FirstOrDefaultAsync(f =>
                    f.FollowerId == follower
                    && f.FollowedToId == followedTo
                    && f.UnfollowDate == null);
        }


        public async Task SetPushTokenAsync(Guid userId, string? token = null)
        {
            var user = await _checkService.FindUserOrThrowExAsync(userId);
            user.PushToken = token;
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetPushTokenAsync(Guid userId)
        {
            var user = await _checkService.FindUserOrThrowExAsync(userId);
            return user.PushToken;
        }
    }
}
