using DataAccess.Entities;
using DataAccess.Repositories;
using Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sugary.WebApi.Models;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("Account")]
[AllowAnonymous]
public class AccountController : BaseController
{
    private readonly UserManager<PortalUser> _userManager;
    private readonly IPasswordHasher<PortalUser> _hasher;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _users;

    public AccountController(UserManager<PortalUser> userManager, 
        IPasswordHasher<PortalUser> hasher, IUserRepository users,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _hasher = hasher;
        _tokenService = tokenService;
        _users = users;
    }
    
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel data)
    {
        var user = await _userManager.FindByEmailAsync(data.Email);
        if (user?.PasswordHash == null)
            return NotFound(new { Success = false, Message = "Hmm, we don't recognize you. Please try again." });

        if (_hasher.VerifyHashedPassword(user, user.PasswordHash, data.Password) == PasswordVerificationResult.Failed)
            return NotFound(new { Success = false, Message = "Hmm, that's not the right Password. Please try again." });

        var token = await _tokenService.CreateToken(user, HttpContext);
        _users.RecordLogin(user.Id);

        return Ok(new
        {
            Success = true,
            Token = token.AccessToken,
            token.RefreshToken,
            token.AccessTokenExpiresAt,
            token.RefreshTokenExpiresAt,
        });
    }
    
    [HttpPost("RefreshToken")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenData)
    {
        var userId = _tokenService.GetUserNameFromToken(tokenData.AccessToken);
        var claimId = _tokenService.GetClaimIdFromToken(tokenData.AccessToken);
        
        if(string.IsNullOrEmpty(userId) || claimId==0)
            return Unauthorized("Invalid access. Please log in again.");
        if (string.IsNullOrEmpty(tokenData.RefreshToken))
            return BadRequest("Refresh Token is required!");
        
        var user = await _userManager.FindByIdAsync(userId);
        if(user==null)
            return Unauthorized("User not found");
        
        var claim = await _users.GetClaimById(claimId);
        if(string.IsNullOrEmpty(claim?.RefreshToken))
            return Unauthorized("Session not found. Kindly login.");
        if(!claim.RefreshToken.Equals(tokenData.RefreshToken))
            return Unauthorized("Invalid Refresh Token!");
        if(DateTime.UtcNow > claim.ExpiryAt)
        {
            await _users.DeleteClaim(claim.Id);
            return Unauthorized("Session is Expired! Please login again.");
        }
        
        var newToken = await _tokenService.RefreshToken(user, claim);
        if(newToken==null) return BadRequest("Failed to log you in!");
        _users.RecordLogin(user.Id);
        
        return Ok(new
        {
            Success = true,
            Token = newToken.AccessToken,
            newToken.RefreshToken,
            newToken.AccessTokenExpiresAt,
            newToken.RefreshTokenExpiresAt,
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Signup")]
    public async Task<IActionResult> Signup([FromBody] RegistrationModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Email);
        if (user != null)
            return NotFound(new { Success = false, Message = "This email already exists" });
        var userEntity = new PortalUser
        {
            UserName = model.Email,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = model.Email,
            Avatar = model.Avatar,
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow,
            Provider = LoginProvider.Email,
        };
        var userCreateResponse = await _userManager.CreateAsync(userEntity, model.Password);
        if (!userCreateResponse.Succeeded)
            return StatusCode(500);
        try
        {
            // _emailService.SendWelcomeMail(model);
        }
        catch (Exception e)
        {
            // ignored
        }
        
        var token = await _tokenService.CreateToken(userEntity, HttpContext);
        if(token==null) return BadRequest("Failed to log you in!");
        _users.RecordLogin(userEntity.Id);
        
        return Created(userEntity.UserName, new
        {
            Success = true,
            Message = "Successfully registered.",
            Token = token.AccessToken,
            token.RefreshToken,
            token.AccessTokenExpiresAt,
            token.RefreshTokenExpiresAt,
            // User = _userDomain.GetLoggedInUser(userEntity)
        });
    }
}