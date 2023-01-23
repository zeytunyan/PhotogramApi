using Api.Models.Comment;
using AutoMapper;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class CommentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly CheckService _checkService;

        public CommentService(IMapper mapper, DataContext context, CheckService checkService)
        {
            _mapper = mapper;
            _context = context;
            _checkService = checkService;
        }

        public async Task<OutCommentModel> CreateCommentAsync(InCommentWithAuthorModel commentToCreate)
        {
            await _checkService.FindUserOrThrowExAsync(commentToCreate.AuthorId);
            await _checkService.FindPostOrThrowExAsync(commentToCreate.PostId);

            var comment = _mapper.Map<Comment>(commentToCreate);
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return _mapper.Map<OutCommentModel>(comment);
        }

        public async Task<OutCommentModel> GetCommentByIdAsync(Guid commentId)
        {
            //// Может ли комментарий существовать, если его автор удален?
            //// Нужно ли проверять существование автора поста?
            var comment = await _context.Comments
                .Include(c => c.Author).ThenInclude(a => a.Avatar)
                .Include(c => c.Post)
                .Include(c => c.Likes)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted && c.Post != null && !c.Post.IsDeleted);

            _checkService.ThrowExIfCommentIsNotExist(comment);

            return _mapper.Map<OutCommentModel>(comment);
        }

        public async Task<List<OutCommentModel>> GetPostCommentsAsync(Guid postId, int skip, int take)
        {
            //// Может ли комментарий существовать, если его автор удален?
            //// Нужно ли проверять существование автора поста?
            await _checkService.FindPostOrThrowExAsync(postId);

            return await _context.Comments
                .Include(c => c.Author).ThenInclude(a => a.Avatar)
                .Include(c => c.Likes)
                .AsNoTracking()
                .OrderByDescending(c => c.Created) // Другие варианты?
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .Skip(skip).Take(take)
                .Select(c => _mapper.Map<OutCommentModel>(c))
                .ToListAsync();
        }


    }
}
