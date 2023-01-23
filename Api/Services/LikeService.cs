using Api.Models.Like;
using Api.Models.User;
using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class LikeService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly CheckService _checkService;

        public LikeService(DataContext context, IMapper mapper, CheckService checkService)
        {
            _context = context;
            _mapper = mapper;
            _checkService = checkService;
        }

        public async Task<bool> PostLikeAsync(LikeModel likeModel)
        {
            var like = await GetPostLikeAsync(likeModel);

            if (like != null)
                like.IsDeleted = true;
            else
                like = await AddPostLikeAsync(likeModel);

            await _context.SaveChangesAsync();
            return !like.IsDeleted;
        }

        public async Task<bool> CommentLikeAsync(LikeModel likeModel)
        {
            var like = await GetCommentLikeAsync(likeModel);

            if (like != null)
                like.IsDeleted = true;
            else
                like = await AddCommentLikeAsync(likeModel);

            await _context.SaveChangesAsync();
            return !like.IsDeleted;
        }


        private async Task<PostLike> AddPostLikeAsync(LikeModel likeModel)
        {
            var postLike = _mapper.Map<PostLike>(likeModel);
            var added = await _context.PostLikes.AddAsync(postLike);
            return added.Entity;
        }

        private async Task<CommentLike> AddCommentLikeAsync(LikeModel likeModel)
        {
            var commentLike = _mapper.Map<CommentLike>(likeModel);
            var added = await _context.CommentLikes.AddAsync(commentLike);
            return added.Entity;
        }


        public async Task<PostLike?> GetPostLikeAsync(LikeModel likeModel)
        {
            // Может ли существовать лайк без пользователя, его поставившего?
            await _checkService.FindUserOrThrowExAsync(likeModel.UserId);
            await _checkService.FindPostOrThrowExAsync(likeModel.ContentId);

            return await _context.PostLikes
                .OrderByDescending(pl => pl.Date)
                .FirstOrDefaultAsync(pl =>
                    pl.UserId == likeModel.UserId 
                    && pl.PostId == likeModel.ContentId 
                    && !pl.IsDeleted);
        }

        public async Task<CommentLike?> GetCommentLikeAsync(LikeModel likeModel)
        {
            // Может ли существовать лайк без пользователя, его поставившего?
            //// Добавить ли проверку на существование поста?
            await _checkService.FindUserOrThrowExAsync(likeModel.UserId);
            await _checkService.FindCommentOrThrowExAsync(likeModel.ContentId);

            return await _context.CommentLikes
                .OrderByDescending(cl => cl.Date)
                .FirstOrDefaultAsync(cl =>
                    cl.UserId == likeModel.UserId 
                    && cl.CommentId == likeModel.ContentId 
                    && !cl.IsDeleted);
        }


        public async Task<List<UserShortInfoModel>> GetPostLikesAsync(Guid postId, int skip, int take)
        {
            // Добавить пользователя и проверку на его существование?
            await _checkService.FindPostOrThrowExAsync(postId);

            //// а если User == null или deleted? 
            return await _context.PostLikes
                .Include(pl => pl.User).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(pl => pl.PostId == postId && !pl.IsDeleted)
                .OrderByDescending(pl => pl.Date)
                .Skip(skip).Take(take)
                .Select(pl => _mapper.Map<UserShortInfoModel>(pl.User))
                .ToListAsync();
        }

        public async Task<List<UserShortInfoModel>> GetCommentLikesAsync(Guid commentId, int skip, int take)
        {
            //// Добавить пост и проверку на его существование?
            // Добавить пользователя и проверку на его существование?
            await _checkService.FindCommentOrThrowExAsync(commentId);

            //// а если User == null или deleted? 

            return await _context.CommentLikes
                .Include(cl => cl.User).ThenInclude(u => u.Avatar)
                .AsNoTracking()
                .Where(cl => cl.CommentId == commentId && !cl.IsDeleted)
                .OrderByDescending(cl => cl.Date)
                .Skip(skip).Take(take)
                .Select(cl => _mapper.Map<UserShortInfoModel>(cl.User))
                .ToListAsync();
        }
    }
}
