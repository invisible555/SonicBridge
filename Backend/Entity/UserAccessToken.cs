using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Entity
{
    public class UserAccessToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        [NotMapped]
        public bool IsActive => !IsRevoked && ExpiryDate > DateTime.UtcNow;
    }
}
