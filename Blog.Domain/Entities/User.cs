using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Entities
{
    [Table("users")]
    public class User : DomainEntity<Guid>
    {

        [Required, MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(255)]
        public string? FullName { get; set; }
        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
