using Blog.Domain;
using Blog.Domain.Interface;
using Blog.Domain.ViewModel.Category;
using Blog.Domain.ViewModel.Post;
using Blog.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using StudentMngt.Api.Controllers.Base;

namespace Blog.API.Controllers
{
    public class HomeController : NoAuthorizeController
    {
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<PostController> _logger;

        public HomeController(IPostService postService, ILogger<PostController> logger, IUserService userService, ICategoryService categoryService) 
        {
            _postService = postService;
            _logger = logger;
            _categoryService = categoryService;
        }

        // GET: api/post/get-all
        [HttpGet]
        [Route("get-all-post")]
        public async Task<ResponseResult<List<PostViewModel>>> GetAllPost()
        {
            try
            {
                var result = await _postService.GetPostPublished();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when get all posts");
                return ResponseResult<List<PostViewModel>>.Fail("Error when get all posts", StatusCodes.Status400BadRequest);
            }
        }

        [HttpGet("get-by-id/{id}")]
          public async Task<ResponseResult<PostViewModel>> GetPostById(Guid id)
            {
                if (id == Guid.Empty)
                {
                    return ResponseResult<PostViewModel>.NotFound();
                }

            var result = await _postService.GetById(id);
                return result;
           }


        [HttpGet]
        [Route("get-all-category")]
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

        //[HttpGet]
        //[Route("get-post")]
        //public async Task<ResponseResult<List<PostViewModel>>> GetPost()
        //{
        //    try
        //    {
        //        var result = await _postService.GetPostPublished();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error when get all posts");
        //        return ResponseResult<List<PostViewModel>>.Fail("Error when get all posts", StatusCodes.Status400BadRequest);
        //    }
        //}

        [HttpGet]
        [Route("get-post")]
        public async Task<ResponseResult<List<PostViewModel>>> GetPost([FromBody] FilterPostModel model)
        {
            try
            {
                var result = await _postService.GetPostPublished(model);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when get all posts");
                return ResponseResult<List<PostViewModel>>.Fail("Error when get all posts", StatusCodes.Status400BadRequest);
            }
        }

    }
}
