using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Domain.Enums;

namespace Blog.Domain.Entities
{
    [Table("posts")]
    public class Post : DomainEntity<Guid>
    {

        [Required, MaxLength(255)]
        public string Title { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Slug { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;   // LONGTEXT / NVARCHAR(MAX)

        public string? Excerpt { get; set; }           // Tóm tắt

        public string? ImageBase64 { get; set; }

        // FK → categories.id
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        public PostStatus Status { get; set; } = PostStatus.Draft;

        public DateTime? PublishedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
