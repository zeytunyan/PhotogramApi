using Api.Models.Comment;
using Api.Models.Like;
using Api.Models.User;
using Api.Services;
using AutoMapper;
using Common;
using Common.Consts;
using Common.Extensions;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CommentService _commentService;
        private readonly CheckService _checkService;
        private readonly LikeService _likeService;

        public CommentController(
            CommentService commentService, 
            CheckService checkService, 
            LikeService likeService,
            LinkGeneratorService links,
            IMapper mapper)
        {
            _likeService= likeService;
            _mapper = mapper;
            _commentService = commentService;
            _checkService = checkService;

            links.LinkAvatarGenerator = x =>
            Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }

        [HttpPost]
        public async Task<OutCommentModel> CreateComment(InCommentModel inComment)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            
            var inCommentWithAuthor = _mapper.Map<InCommentWithAuthorModel>(inComment);
            inCommentWithAuthor.AuthorId = userId;

            return await _commentService.CreateCommentAsync(inCommentWithAuthor);
        }

        [HttpGet]
        public async Task<OutCommentModel> GetComment(Guid commentId)
        {
            return await _commentService.GetCommentByIdAsync(commentId);
        }

        [HttpGet]
        public async Task<List<OutCommentModel>> GetPostComments(Guid postId, int skip = 0, int take = 10)
        {
            return await _commentService.GetPostCommentsAsync(postId, skip, take);
        }

        [HttpPost]
        public async Task<bool> CommentLike(Guid commentId)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);

            var likeModel = new CommentLikeModel() 
            { 
                UserId = userId, 
                ContentId = commentId
            };

            return await _likeService.CommentLikeAsync(likeModel);
        }

        [HttpGet]
        public async Task<bool> CheckLike(Guid commentId)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);

            var checkRes = await _likeService.GetCommentLikeAsync(new() 
            { 
                UserId = userId, 
                ContentId = commentId 
            });

            return checkRes != null;
        }

        [HttpGet]
        public async Task<List<UserShortInfoModel>> GetCommentLikes(Guid commentId, int skip = 0, int take = 10)
        {
            return await _likeService.GetCommentLikesAsync(commentId, skip, take);
        }
    }
}
