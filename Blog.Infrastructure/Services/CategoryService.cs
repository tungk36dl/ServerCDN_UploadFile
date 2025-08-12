using Blog.Domain;
using Blog.Domain.Entities;
using Blog.Domain.Interface;
using Blog.Domain.ViewModel.Category;
using Microsoft.EntityFrameworkCore;
using StudentMngt.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Repository
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category, Guid> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;   
       
        public CategoryService(IGenericRepository<Category, Guid> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseResult> Create(CreateCategoryViewModel model)
        {
            var Category = new Category()
            {
                Id = new Guid(),
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug,
                CreatedAt = DateTime.Now,
                UpdatedAt =  DateTime.Now
            };
            _categoryRepository.Add(Category);
            await _unitOfWork.SaveChange();
            return ResponseResult.Success("Create category successfully");

        }

        public async Task<ResponseResult> Delete(Guid id)
        {
            var category = await _categoryRepository.FindByIdAsync(id);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            _categoryRepository.Remove(category);
            await _unitOfWork.SaveChange();
            return ResponseResult.Success("Delete category successfully");

        }

        public async Task<ResponseResult<List<CategoryViewModel>>> GetAll()
        {
            var categories = _categoryRepository.FindAll(null).Select(c => 
            new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Slug = c.Slug,
            });
            var result = await categories.ToListAsync();
            return ResponseResult.Success(result);
        }

        public async Task<CategoryViewModel> GetById(Guid id)
        {
            var category = await _categoryRepository.FindByIdAsync(id);
            if (category == null)
            {
                throw new Exception("Not found category");
            }
            var result = new CategoryViewModel()
            {
                Id = id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
            };
            return result;
        }

        public async Task<ResponseResult> Update(UpdateCategoryViewModel model)
        {
            var category = await _categoryRepository.FindByIdAsync(model.Id);
            if (category == null)
            {
                throw new Exception("Category not found!!");
            }
            category.Name = model.Name;
            category.Description = model.Description;
            category.Slug = model.Slug;
            category.UpdatedAt = DateTime.Now;
            _categoryRepository.Update(category);
            await _unitOfWork.SaveChange();
            return ResponseResult.Success("Update category successfully");

        }
    }
}
