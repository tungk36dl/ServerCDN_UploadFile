using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.ViewModel.Post
{
    public class CreatePostViewModel
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public string CoverImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public string Status { get; set; }
    }
}
