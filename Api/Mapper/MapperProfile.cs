using Api.Mapper.MapperActions;
using Api.Models.Attach;
using Api.Models.Comment;
using Api.Models.Like;
using Api.Models.Post;
using Api.Models.User;
using AutoMapper;
using Common;
using DAL.Entities;

namespace Api.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<InFileMetaModel, Avatar>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));

            CreateMap<UserRegistrationModel, User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime));

            // Здесь с помощью AfterMap мапится из User'а, у которого ссылка на физическую аватарку,
            // в UserPageInformationModel, у которого ссылка на метод в Api
            CreateMap<User, UserPageInfoModel>()
                .ForMember(d => d.PostsCount, m => m.MapFrom(s => s.Posts != null ? s.Posts.Count(p => !p.IsDeleted) : 0))
                //// Если Follower удален?
                .ForMember(d => d.FollowersCount, m => m.MapFrom(s => s.Followers != null ? s.Followers.Count(f => f.UnfollowDate == null) : 0))
                //// Если FollowedTo удален?
                .ForMember(d => d.FollowingsCount, m => m.MapFrom(s => s.Followings != null ? s.Followings.Count(f => f.UnfollowDate == null) : 0))
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<User, UserFullInfoModel>()
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<User, UserShortInfoModel>()
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<User, UserSettingsModel>();

            CreateMap<Avatar, AttachModel>();

            CreateMap<Post, OutPostModel>()
                .ForMember(d => d.RepostsCount, m => m.MapFrom(s => s.Reposts != null ? s.Reposts.Count(r => !r.IsDeleted) : 0))
                .ForMember(d => d.LikesCount, m => m.MapFrom(s => s.Likes != null ? s.Likes.Count(l => !l.IsDeleted) : 0))
                .ForMember(d => d.CommentsCount, m => m.MapFrom(s => s.Comments != null ? s.Comments.Count(c => !c.IsDeleted) : 0));

            CreateMap<InPostModel, Post>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));


            CreateMap<IFormFile, InFileMetaModel>()
                .ForMember(d => d.TempId, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.FileName))
                .ForMember(d => d.MimeType, m => m.MapFrom(s => s.ContentType))
                .ForMember(d => d.Size, m => m.MapFrom(s => s.Length));
            
            CreateMap<InFileMetaModel, PostContent>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));
            
            CreateMap<PostContent, AttachModel>();

            // Здесь с помощью AfterMap мапится из PostConent, у которого ссылка на физический файл
            // в AttachExtetnalContent, у которого ссылка на метод в Api,
            // чтобы когда пост отправлялся, файлы бы не отправлялись, но клиент мог запросить их по ссылке
            CreateMap<PostContent, AttachExternalModel>()
                .AfterMap<PostContentMapperAction>();

            //CreateMap<Following, FollowingModel>();

            CreateMap<InCommentModel, InCommentWithAuthorModel>();
                
            CreateMap<InCommentWithAuthorModel, Comment>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));

            CreateMap<Comment, OutCommentModel>()
                .ForMember(d => d.ChildrenCount, m => m.MapFrom(s => s.Children != null ? s.Children.Count(c => !c.IsDeleted) : 0))
                .ForMember(d => d.LikesCount, m => m.MapFrom(s => s.Likes != null ? s.Likes.Count(l => !l.IsDeleted) : 0));

            CreateMap<LikeModel, PostLike>()
                .ForMember(d => d.Date, m => m.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.PostId, m => m.MapFrom(s => s.ContentId));

            CreateMap<LikeModel, CommentLike>()
                .ForMember(d => d.Date, m => m.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.CommentId, m => m.MapFrom(s => s.ContentId));
        }
    }
}
