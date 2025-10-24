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
        public string Username {  get; set; }
        [Required]
        public string Password {  get; set; }
        public string HoTen {  get; set; }
        public string Email {  get; set; }
        public string Sdt {  get; set; }
        public string DiaChi {  get; set; }

        public ICollection<UserRole> UserRoles { get; set; }=new List<UserRole>();
    }
}
