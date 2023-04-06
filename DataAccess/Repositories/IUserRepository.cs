using DataAccess.Entities;
using Framework.Models;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<PortalUser?> Get(string id);
        Task<PortalUser?> GetByVerifiedEmail(string email);
        void RecordLogin(string userId);
        Task<List<string>> GetFcmTokens(string userId);
        Task<List<UserClaim>> GetClaims(string userId);
        Task<UserClaim?> GetClaimById(int claimId);
        Task<ResponseModel> SaveOrUpdateClaim(UserClaim entity);
        Task<ResponseModel> DeleteClaim(int id);
        Task<UserClaim?> GetClaimByDeviceId(string userId, string deviceId);
    }
}
