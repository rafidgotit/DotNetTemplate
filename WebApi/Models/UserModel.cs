using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Sugary.WepApi.Models
{
    public class UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       
        [JsonProperty("PhoneNumber")]
        [Required]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public int? RoleId { get; set; }
    }

    public class PasswordChangeModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class PasswordResetModel
    {
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int Otp { get; set; }
    }
}
