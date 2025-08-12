using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.ViewModel.Auth
{
    public class LoginResultViewModel
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireAt { get; set; }
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
