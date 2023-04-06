using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Sugary.WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiExceptionFilter]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class BaseController : Controller
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        protected BaseController()
        {
        }

        protected string UserId => User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
        protected int ClaimId => int.Parse(User.Claims.First(x => x.Type == CustomClaimTypes.ClaimId).Value);
        protected int? RoleId => User.Identity==null ? null : int.Parse(User.Identity.GetRoleId());
    }

    public static class CustomClaimTypes
    {
        public const string RoleId = "RoleId";
        public const string IsCustomer = "IsCustomer";
        public const string IsGuest = "IsGuest";
        public const string ClaimId = "ClaimId";
    }
    public static class IdentityExtensions
    {
        public static string GetRoleId(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.RoleId);

            if (claim == null)
                return string.Empty;

            return claim?.Value ?? string.Empty;
        }
    }

}