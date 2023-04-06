using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataAccess.Entities;
using DataAccess.Repositories;
using Framework;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Sugary.WebApi.Models;
using WebApi.Controllers;

namespace WebApi.Services;

public interface ITokenService
{
    Task<TokenModel?> CreateToken(PortalUser user, HttpContext context);
    string? GetUserNameFromToken(string token);
    int GetClaimIdFromToken(string token);
    Task<TokenModel?> RefreshToken(PortalUser user, UserClaim claim);
    DeviceInfoModel GetDeviceInfoFromRequest(HttpContext context);
    int GetRoleIdFromToken(string token);
}

public class TokenService : ITokenService
{
    private readonly UserManager<PortalUser> _userManager;
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;
    private readonly ICipherService _cipherService;
    public TokenService(UserManager<PortalUser> userManager, IConfiguration config, ICipherService cipherService,
        IUserRepository users)
    {
        _userManager = userManager;
        _config = config;
        _cipherService = cipherService;
        _users = users;
    }

    public string? GetUserNameFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        var jwtToken = new JwtSecurityToken(token);
        return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
    }

    public int GetRoleIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token)) throw new Exception("Token is empty");
        var jwtToken = new JwtSecurityToken(token);
        var roleValue = jwtToken.Claims.First(x => x.Type == CustomClaimTypes.RoleId).Value;
        return int.Parse(_cipherService.Decrypt(roleValue));
    }

    public int GetClaimIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token)) throw new Exception("Token is empty");
        var jwtToken = new JwtSecurityToken(token);
        var value = jwtToken.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.ClaimId)?.Value;
        return value == null ? 0 : int.Parse(value);
    }
        
    public DeviceInfoModel GetDeviceInfoFromRequest(HttpContext context)  
    {
        var agent = context.Request.Headers["User-Agent"];
            
        return new DeviceInfoModel
        {
            DeviceId = agent!,
        };
    }

    public async Task<TokenModel?> CreateToken(PortalUser user, /*DeviceInfoModel? deviceInfo,*/ HttpContext context)
    {
        var deviceInfo = GetDeviceInfoFromRequest(context);
        var existingClaim = await _users.GetClaimByDeviceId(user.Id, deviceInfo.DeviceId);
        if (existingClaim != null) await _users.DeleteClaim(existingClaim.Id);
            
        var refreshTokenData = GenerateRefreshToken();
        var newClaim = CreateClaim(user, deviceInfo, refreshTokenData);
        await _users.SaveOrUpdateClaim(newClaim);
            
        var accessTokenData = await GenerateAccessToken(user, newClaim);
        return new TokenModel
        {
            AccessToken = accessTokenData.AccessToken,
            AccessTokenExpiresAt = accessTokenData.AccessTokenExpiresAt,
            RefreshToken = refreshTokenData.RefreshToken,
            RefreshTokenExpiresAt = refreshTokenData.RefreshTokenExpiresAt
        };
    }

    public async Task<TokenModel?> RefreshToken(PortalUser user, UserClaim claim)
    {
        var refreshTokenData = GenerateRefreshToken();
        claim.ReclaimedAt = DateTime.UtcNow;
        claim.RefreshToken = refreshTokenData.RefreshToken;
        claim.ExpiryAt = refreshTokenData.RefreshTokenExpiresAt ?? DateTime.UtcNow.AddDays(ApplicationConstant.RefreshTokenExpiryDay);
        var result = await _users.SaveOrUpdateClaim(claim);
        if (!result.Success) return null;
            
        var accessTokenData = await GenerateAccessToken(user, claim);
        return new TokenModel
        {
            AccessToken = accessTokenData.AccessToken,
            AccessTokenExpiresAt = accessTokenData.AccessTokenExpiresAt,
            RefreshToken = refreshTokenData.RefreshToken,
            RefreshTokenExpiresAt = refreshTokenData.RefreshTokenExpiresAt
        };
    }
        
    private UserClaim CreateClaim(PortalUser user, DeviceInfoModel deviceInfo, TokenModel refreshTokenData)
    {
        var now = DateTime.UtcNow;
        return new UserClaim
        {
            Id = 0,
            UserId = user.Id,
            CreatedAt = now,
            ReclaimedAt = now,
            DeviceId = deviceInfo.DeviceId,
            Platform = deviceInfo.Platform,
            BrandName = deviceInfo.BrandName,
            ModelName = deviceInfo.ModelName,
            IpAddress = deviceInfo.IpAddress,
            ExpiryAt = refreshTokenData.RefreshTokenExpiresAt ?? now.AddDays(ApplicationConstant.RefreshTokenExpiryDay),
            FcmToken = deviceInfo.FcmToken,
            RefreshToken = refreshTokenData.RefreshToken,
            SystemId = deviceInfo.SystemId,
            SystemVersion = deviceInfo.SystemVersion,
            ClaimType = user.IsAdmin ? "Admin" : "Customer",
            ClaimValue = user.RoleId?.ToString() ?? deviceInfo.DeviceId,
        };
    }

    private async Task<TokenModel> GenerateAccessToken(PortalUser user, UserClaim claim)
    {
        var roleId = user.RoleId ?? 0;
            
        var userClaims = await _userManager.GetClaimsAsync(user);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            user.Email is null? null : new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sid, user.Id),
            new Claim(CustomClaimTypes.RoleId, _cipherService.Encrypt(roleId.ToString())),
            new Claim(CustomClaimTypes.ClaimId, claim.Id.ToString()),
        }.Union(userClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.Now.AddDays(ApplicationConstant.AccessTokenExpiryDay);

        var token = new JwtSecurityToken(
            issuer: _config["Tokens:Issuer"],
            audience: _config["Tokens:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );
        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenModel
        {
            AccessToken = generatedToken,
            AccessTokenExpiresAt = expiresAt,
        };
    }
        
    private TokenModel GenerateRefreshToken()
    {
        const int length = 80;
        var random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
        var token = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
            
        var expiresAt = DateTime.Now.AddDays(ApplicationConstant.RefreshTokenExpiryDay);
            
        return new TokenModel
        {
            RefreshToken = token,
            RefreshTokenExpiresAt = expiresAt
        };
    }
}