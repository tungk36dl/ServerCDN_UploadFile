using Blog.Domain.Entities;
using Blog.Domain.ViewModel.Category;
using StudentMngt.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Interface
{
    public interface ICategoryService 
    {
        Task<ResponseResult<List<CategoryViewModel>>> GetAll();
        Task<CategoryViewModel> GetById(Guid id);
        Task<ResponseResult> Update(UpdateCategoryViewModel model);
        Task<ResponseResult> Create(CreateCategoryViewModel model);
        Task<ResponseResult> Delete(Guid id);
    }
}
