using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Entities.UserInfo
{
    [Table("user")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string HoTen { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Sdt { get; set; } = string.Empty;

        public string DiaChi { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
