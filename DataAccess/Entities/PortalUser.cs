using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    [Table("PortalUser")]
    public class PortalUser : IdentityUser
    {
        [Key]
        public override string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? SuperAdmin { get; set; }
       
        public string Avatar { get; set; }
        public string Provider { get; set; }
        
        public int? RoleId { get; set; }
        public string GoogleUserId { get; set; }
        public string FacebookUserId { get; set; }
        public string AppleUserId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? Otp { get; set; }
        public DateTime? OtpExpiryAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
