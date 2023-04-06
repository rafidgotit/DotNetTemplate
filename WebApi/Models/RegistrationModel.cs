using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class RegistrationModel
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Avatar { get; set; }

    public string Email { get; set; }
}