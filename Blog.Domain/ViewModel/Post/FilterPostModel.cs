using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.ViewModel.Post
{
    public class FilterPostModel
    {
        public string? SearchValue { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
