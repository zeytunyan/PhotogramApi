using DAL.Entities;

namespace Api.Models.User
{
    public class UserSettingsModel
    {
        public Guid Id { get; set; }
        public bool IsPrivate { get; set; } = false;
    }
}
