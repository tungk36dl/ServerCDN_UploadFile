using Blog.Domain;
using Blog.Domain.Interface;
using Blog.Domain.ViewModel.Post;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [ApiController]
    [Route("api/post")]
    public class PostController : AuthorizeController
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger, IUserService userService) : base(userService)
        {
            _postService = postService;
            _logger = logger;
        }

        // POST: api/post/create
        [HttpPost]
        [Route("create")]
        public async Task<ResponseResult> CreatePost([FromBody] CreatePostViewModel model)
        {
            var result = await _postService.Create(model);
            return result;
        }

        // GET: api/post/get-all
        [HttpGet]
        [Route("get-all")]
        public async Task<ResponseResult<List<PostViewModel>>> GetAllPost()
        {
            try
            {
                var result = await _postService.GetAll();
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

        // POST: api/post/update
        [HttpPost]
        [Route("update")]
        public async Task<ResponseResult> UpdatePost([FromBody] UpdatePostViewModel model)
        {
            var result = await _postService.Update(model);
            return result;
        }

  

        // DELETE: api/post/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<ResponseResult> DeletePost(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new Exception("Id is required");
            }

            var result = await _postService.Delete(id);
            return result;
        }
    }
}
