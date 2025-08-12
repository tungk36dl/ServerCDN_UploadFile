using Blog.Domain.Entities;
using Blog.Domain.ViewModel.Auth;
using Blog.Domain.ViewModel.User;
using StudentMngt.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Interface
{
    public interface IUserService 
    {
        Task<ResponseResult<List<UserViewModel>>> GetAll();
        Task<ResponseResult<UserViewModel>> GetById(Guid id);
        Task<ResponseResult> Create(CreateUserViewModel model);
        Task<ResponseResult> Update(UpdateUserViewModel model);
        Task<ResponseResult> Delete(Guid id);
        Task<ResponseResult> LoginAsync(LoginUserViewModel model);
        Task<User> FindUserById(Guid id); 
    }
}
