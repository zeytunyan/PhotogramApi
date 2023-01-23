using System.ComponentModel.DataAnnotations;

namespace Api.Models.User
{
    public class UserRegistrationModel
    {
        [Required(ErrorMessage = "No nickname specified")]
        public string NickName { get; set; }

        [Required(ErrorMessage = "No email address specified")]
        [EmailAddress(ErrorMessage = "Incorrect email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "No password specified")]
        [RegularExpression(
            @"(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}", 
            ErrorMessage = "The password must contain at least 8 characters with at least one digit, one uppercase and one lowercase letter")]
        public string Password { get; set; }

        [Required(ErrorMessage = "No password retry specified")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match")]
        public string RetryPassword { get; set; } 

        [Required(ErrorMessage = "No birth date specified")]
        public DateTimeOffset BirthDate { get; set; }

        [Required(ErrorMessage = "No phone number specified")]
        [Phone(ErrorMessage = "Incorrect phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "No given name specified")]
        public string GivenName { get; set; } 
        
        public string? Surname { get; set; } = "";

        [Required(ErrorMessage = "No country specified")]
        [RegularExpression(@"[a-zA-Z]{2,}", ErrorMessage = "Incorrect country name")]
        public string Country { get; set; } 

        [Required(ErrorMessage = "No country specified")]
        [RegularExpression(
            @"^(Man|Woman|Another|man|woman|another)$",
            ErrorMessage = "Incorrect gender (must be specified as one of three values: 'Man', 'Woman', 'Another'")]
        public string Gender { get; set; }


        public UserRegistrationModel(
            string nickName, 
            string email, 
            string password, 
            string retryPassword, 
            DateTimeOffset birthDate, 
            string phoneNumber, 
            string givenName, 
            string? surname, 
            string country, 
            string gender)
        {
            NickName = nickName;
            Email = email;
            Password = password;
            RetryPassword = retryPassword;
            BirthDate = birthDate;
            PhoneNumber = phoneNumber;
            GivenName = givenName;
            Surname = surname ?? "";
            Country = country;
            Gender = gender;
        }
    }
}
