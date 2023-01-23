namespace Api.Models.User
{
    public class UserInfoModel
    {
        public Guid Id { get; set; }
        public string NickName { get; set; } = null!;
        public string? AvatarLink { get; set; }
    }
}
