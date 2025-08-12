using Blog.Domain;
using Blog.Domain.Entities;
using Blog.Domain.Interface;
using Blog.Domain.ViewModel.Category;
using Blog.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [ApiController]
    public class CategoryController : AuthorizeController
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, IUserService userService) : base(userService)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ResponseResult> CreateCategory([FromBody] CreateCategoryViewModel model)
        {
            var result = await _categoryService.Create(model);
            return result;
        }

        [HttpGet]
        [Route("get-all")]
        public async Task<ResponseResult<List<CategoryViewModel>>> GetAllCategory()
        {
            try
            {
                var result = await _categoryService.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<ResponseResult> UpdateCategory([FromBody] UpdateCategoryViewModel model)
        {
            var result = await _categoryService.Update(model);
            return result;
        }


        [HttpGet("get-by-id/{id}")]
        public async Task<CategoryViewModel> GetCategoryById(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new Exception("Id is required");
            }
            var result = await _categoryService.GetById(id);
            return result;
        }

        // DELETE: api/category/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<ResponseResult> DeleteCategory(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new Exception("Id is required");
            }

            var result = await _categoryService.Delete(id);
            return result;
        }
    }
}
