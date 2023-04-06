using System;
using System.ComponentModel.DataAnnotations;

namespace Sugary.WebApi.Models;

public class LoginModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}

public class ProfileUpdateDto
{
    public string Avatar { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateTime? Birthday { get; set; }
    public string Personality { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Gender { get; set; }
}
public class SocialAuthRequest
{
    [Required]
    public string Provider { get; set; }
    [Required]
    public string Token { get; set; }
        
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DeviceInfoModel DeviceInfo { get; set; }
}
    
public class SocialUser
{
    public string Id { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }
    public string Name { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Picture { get; set; }
    public string Locale { get; set; }
    public string Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public string Provider { get; set; }
}

public class TokenModel
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? AccessTokenExpiresAt { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
}