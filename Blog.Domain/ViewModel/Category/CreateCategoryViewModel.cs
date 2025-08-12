using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.ViewModel.Category
{
    public class CreateCategoryViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug  { get; set; }
    }
}
