using Blog.Domain.Interface;
using Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;
        private IDbContextTransaction? _transaction;
        //public IUserRepository UserRepository { get; }
        //public IPostRepository PostRepository { get; }
        //public ICategoryRepository CategoryRepository { get; }

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task CreateTransaction()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            if (_transaction == null)
            {
                throw new Exception("");
            }
            await _transaction.CommitAsync();
        }

        public async Task Rollback()
        {
            if (_transaction == null)
            {
                throw new Exception("");
            }
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        public async Task SaveChange()
        {
            await _context.SaveChangesAsync();
        }

        public async void Dispose()
        {
            await _context.DisposeAsync();
        }

    }
}
