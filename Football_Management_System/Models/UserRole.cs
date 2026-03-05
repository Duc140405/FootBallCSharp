using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Management_System.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public Role()
        {
            Users = new HashSet<User>();
        }
    }

    [Table("Users")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        public int RoleID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}
