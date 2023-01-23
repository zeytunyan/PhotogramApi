using Api.Models.Attach;
using Api.Models.Like;
using Api.Models.Post;
using Api.Models.User;
using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly CheckService _checkService;
        
        public PostService(CheckService checkService, IMapper mapper, DataContext context)
        {
            _checkService = checkService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<Guid/*OutPostModel*/> CreatePostAsync(InPostModel model, List<string> paths)
        {
            var dbPostModel = _mapper.Map<Post>(model);

            foreach (var metaWithPath in dbPostModel.Contents!.Zip(paths))
            {
                metaWithPath.First.AuthorId = dbPostModel.AuthorId;
                metaWithPath.First.PostId = dbPostModel.Id;
                metaWithPath.First.FilePath = metaWithPath.Second;
            }

            await _context.Posts.AddAsync(dbPostModel);
            await _context.SaveChangesAsync();

            return dbPostModel.Id;//mapper.Map<OutPostModel>(dbPostModel); // Там нет Author'a, только AuthorId, чтобы был, надо вытаскивать из базы
        }

        public async Task<List<OutPostModel>> GetAllPostsAsync(int skip, int take)
        {
            var posts = await _context.Posts
                .Include(p => p.Author).ThenInclude(u => u.Avatar)
                .Include(p => p.Contents)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                //.Include(x => x.Reposts)
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.Created)
                .Skip(skip).Take(take)
                .Select(p => _mapper.Map<OutPostModel>(p))
                .ToListAsync();

            return posts;
        }

        public async Task<OutPostModel> GetPostAsync(Guid id)
        {
            //// Может ли пост существовать, если его автор удален?
            var post = await _context.Posts
                  .Include(p => p.Author).ThenInclude(u => u.Avatar)
                  .Include(p => p.Contents)
                  .Include(p => p.Likes)
                  .Include(p => p.Comments)
                  //.Include(x => x.Reposts)
                  .AsNoTracking()
                  .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            _checkService.ThrowExIfPostIsNotExist(post);

            return _mapper.Map<OutPostModel>(post);
        }

        public async Task<List<OutPostModel>> GetPostsAsync(Guid userId, int skip, int take)
        {
            //// Может ли пост существовать, если его автор удален?
            await _checkService.FindUserOrThrowExAsync(userId);

            return await _context.Posts
                .Include(p => p.Author).ThenInclude(u => u.Avatar)
                .Include(p => p.Contents)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                //.Include(x => x.Reposts)
                .AsNoTracking()
                .Where(p => p.AuthorId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.Created)
                .Skip(skip).Take(take)
                .Select(p => _mapper.Map<OutPostModel>(p))
                .ToListAsync();
        }

        public async Task<List<OutPostModel>> GetFeedAsync(Guid userId, int skip, int take)
        {
            await _checkService.FindUserOrThrowExAsync(userId);

            var followings = _context.Followings
                .Include(f => f.FollowedTo)
                .Where(f =>
                    f.FollowerId == userId
                    && f.UnfollowDate == null
                    && f.FollowedTo != null
                    && !f.FollowedTo.IsDeleted)
                .AsNoTracking();

            // Здесь не берутся посты, автор которых удален
            var posts = _context.Posts
                .Include(p => p.Author).ThenInclude(u => u.Avatar)
                .Include(p => p.Contents)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                //.Include(p => p.Reposts)
                .Where(p => !p.IsDeleted && p.Author != null && !p.Author.IsDeleted)
                .AsNoTracking();

            var feed = await followings.Join(posts, 
                f => f.FollowedToId, 
                p => p.AuthorId, 
                (f, p) => p)
                .AsNoTracking()
                .OrderByDescending(p => p.Created)
                .Skip(skip).Take(take)
                .Select(p => _mapper.Map<OutPostModel>(p))
                .ToListAsync();

            return feed;
        }

        public async Task<AttachModel> GetPostContentAsync(Guid postContentId)
        {
            var postContent = await _checkService.FindPostContentOrThrowExAsync(postContentId);
            return _mapper.Map<AttachModel>(postContent);
        }
    }
}
