namespace Api.Models.User
{
    public class UserPageInfoModel : UserInfoModel
    {
        public string FullName { get; set; } = null!;
        public string? Profession { get; set; }
        public string? Status { get; set; }

        public int PostsCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingsCount { get; set; }
    }
}
