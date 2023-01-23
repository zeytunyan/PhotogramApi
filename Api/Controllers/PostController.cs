using Api.Models.Like;
using Api.Models.Post;
using Api.Models.User;
using Api.Services;
using Common;
using Common.Consts;
using Common.Extensions;
using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly FileService _fileService;
        private readonly CheckService _checkService;
        private readonly LikeService _likeService;

        public PostController(
            PostService postService, 
            FileService fileService, 
            LinkGeneratorService links, 
            CheckService checkService,
            LikeService likeService)
        {
            _postService = postService;
            _fileService = fileService;
            _checkService = checkService;
            _likeService = likeService;  

            links.LinkContentGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostContent), new
            {
                postContentId = x.Id,
            });
            links.LinkAvatarGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<OutPostModel>> GetAllPosts(int skip = 0, int take = 10)
            => await _postService.GetAllPostsAsync(skip, take);

        [HttpGet]
        public async Task<List<OutPostModel>> GetCurrentUserPosts(int skip = 0, int take = 10)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await GetPosts(userId, skip, take);
        }

        [HttpGet]
        public async Task<List<OutPostModel>> GetPosts(Guid userId, int skip = 0, int take = 10)
        {
            return await _postService.GetPostsAsync(userId, skip, take);
        }

        [HttpGet]
        public async Task<List<OutPostModel>> GetFeed(int skip = 0, int take = 10)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await _postService.GetFeedAsync(userId, skip, take);
        }

        [HttpGet]
        public async Task<OutPostModel> GetPost(Guid id)
        {
            return await _postService.GetPostAsync(id);
        }

        [HttpPost]
        public async Task<Guid/*OutPostModel*/> CreatePost(InPostModel request)
        {
            _checkService.ThrowExIfPostContentIsEmpty(request);

            if (!request.AuthorId.HasValue)
                request.AuthorId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            
            var filePaths = new List<string>();

            foreach (var file in request.Contents)
                filePaths.Add(_fileService.MoveToPermanentFolder(file.TempId.ToString()));
            
            return await _postService.CreatePostAsync(request, filePaths);
        }

        [HttpPost]
        public async Task<bool> PostLike(Guid postId)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);

            var likeModel = new PostLikeModel()
            {
                UserId = userId,
                ContentId = postId
            };

            return await _likeService.PostLikeAsync(likeModel);
        }

        [HttpGet]
        public async Task<bool> CheckLike(Guid postId)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);

            var checkRes = await _likeService.GetPostLikeAsync(new()
            {
                UserId = userId,
                ContentId = postId
            });

            return checkRes != null;
        }

        [HttpGet]
        public async Task<List<UserShortInfoModel>> GetPostLikes(Guid postId, int skip = 0, int take = 10)
        {
            return await _likeService.GetPostLikesAsync(postId, skip, take);
        }

    }
}
