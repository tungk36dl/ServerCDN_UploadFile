using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        //public IUserRepository UserRepository { get; }
        //public IPostRepository PostRepository { get; }
        //public ICategoryRepository CategoryRepository { get; }

        Task CreateTransaction();
        Task Commit();
        Task Rollback();
        Task SaveChange();
    }
}
