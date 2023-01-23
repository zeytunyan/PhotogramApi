using DAL.Entities;

namespace Api.Models.User
{
    public class UserFullInfoModel : UserInfoModel 
    {
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string GivenName { get; set; } = null!;
        public string? Surname { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public string? Profession { get; set; }
        public string? Status { get; set; }
        public string Country { get; set; } = null!;
        public string Gender { get; set; } = null!;
    }
}
