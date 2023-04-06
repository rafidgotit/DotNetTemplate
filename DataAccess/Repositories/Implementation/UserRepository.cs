using DataAccess.Entities;
using Framework.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly PortalDbContext _dbContext;
        public UserRepository(PortalDbContext context)
        {
            _dbContext = context;
        }
        public async Task<PortalUser?> Get(string id)
        {
            return await _dbContext.PortalUser.FirstOrDefaultAsync(x=>x.Id==id);
        }

        public async Task<PortalUser?> GetByVerifiedEmail(string email)
        {
            return await _dbContext.PortalUser.Where(x=> x.EmailConfirmed).FirstOrDefaultAsync(x=>x.Email==email);
        }

        public void RecordLogin(string userId)
        {
            var user = _dbContext.PortalUser.FirstOrDefault(x => x.Id == userId);
            if (user == null) return;
            user.LastLoginAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
        }

        public async Task<List<string>> GetFcmTokens(string userId)
        {
            return await _dbContext.UserClaim.Where(x => x.UserId == userId).Select(x=> x.FcmToken).ToListAsync();
        }
        
        public async Task<List<UserClaim>> GetClaims(string userId)
        {
            return await _dbContext.UserClaim.Where(x => x.UserId == userId).ToListAsync();
        }
        
        public async Task<UserClaim?> GetClaimById(int claimId)
        {
            return await _dbContext.UserClaim.FirstOrDefaultAsync(x => x.Id == claimId);
        }
        
        public async Task<UserClaim?> GetClaimByDeviceId(string userId, string deviceId)
        {
            return await _dbContext.UserClaim.FirstOrDefaultAsync(x => x.UserId == userId && x.DeviceId == deviceId);
        }
        
        public async Task<ResponseModel> SaveOrUpdateClaim(UserClaim entity)
        {
            if (entity.Id == 0)
            {
                _dbContext.Add(entity);
            } else
            {
                _dbContext.Update(entity);
            }
            var result = await _dbContext.SaveChangesAsync();
            return new ResponseModel { Success = result > 0 };
        }
        
        public async Task<ResponseModel> DeleteClaim(int id)
        {
            var entity = await _dbContext.UserClaim.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return new ResponseModel { Success = true };
            _dbContext.Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return new ResponseModel { Success = result > 0 };
        }
    }
}
