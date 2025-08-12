using Blog.Domain;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Blog.Domain.Interface;
using Blog.Domain.ViewModel.Post;
using Microsoft.EntityFrameworkCore;
using StudentMngt.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Repository
{
    /// <summary>
    /// Service xử lý CRUD cho bảng <c>posts</c>.
    /// </summary>
    public class PostService : IPostService
    {
        private readonly IGenericRepository<Post, Guid> _postRepository;
        private readonly IGenericRepository<Category, Guid> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostService(
            IGenericRepository<Post, Guid> postRepository,
            IGenericRepository<Category, Guid> categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        #region CREATE
        public async Task<ResponseResult> Create(CreatePostViewModel model)
        {
            // Kiểm tra Category tồn tại (nếu truyền Id)
            if (model.CategoryId != Guid.Empty)
            {
                var categoryExist = await _categoryRepository.FindByIdAsync(model.CategoryId);
                if (categoryExist is null)
                    throw new Exception("Category not found");
            }

            if (!PostStatusHelper.TryParse(model.Status, out var postStatus))
            {
                 throw new Exception($"Status “{model.Status}” không hợp lệ.");
            }
            var isPublished = false;
            if (postStatus == PostStatus.Published) {
                isPublished = true;
            }

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = model.Title.Trim(),
                Slug = model.Slug?.Trim(),
                Content = model.Content,
                Excerpt = model?.Excerpt,
                ImageBase64 = model?.CoverImageUrl,
                CategoryId = model.CategoryId,
                Status = postStatus,
                PublishedAt = isPublished ? DateTime.Now : null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _postRepository.Add(post);
            await _unitOfWork.SaveChange();
            return ResponseResult.Success("Create post successfully");
        }
        #endregion

        #region DELETE
        public async Task<ResponseResult> Delete(Guid id)
        {
            var post = await _postRepository.FindByIdAsync(id);
            if (post is null)
                throw new Exception("Post not found");

            _postRepository.Remove(post);
            await _unitOfWork.SaveChange();
            return ResponseResult.Success("Delete post successfully");
        }
        #endregion

        #region GET-ALL
        public async Task<ResponseResult<List<PostViewModel>>> GetAll()
        {
            // Include Category để lấy CategoryName
            var posts =  _postRepository.FindAll(null, p => p.Category).Select(p => new PostViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                Content = p.Content,
                CoverImageUrl = p.ImageBase64,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                CategoryId = p.CategoryId.Value,
                Status = p.Status.ToString(),
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });



            var result =  await posts.ToListAsync();
            return ResponseResult.Success(result);
        }
        #endregion

        #region GET-POST-PUBLISHED
        public async Task<ResponseResult<List<PostViewModel>>> GetPostPublished()
        {
            // Include Category để lấy CategoryName
            var posts = _postRepository.FindAll(p=> p.Status == PostStatus.Published, p => p.Category).Select(p => new PostViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                Content = p.Content,
                CoverImageUrl = p.ImageBase64,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                CategoryId = p.CategoryId.Value,
                Status = p.Status.ToString(),
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });



            var result = await posts.ToListAsync();
            return ResponseResult.Success(result);
        }
        #endregion


        #region GET-POST
        public async Task<ResponseResult<List<PostViewModel>>> GetPostPublished(FilterPostModel model)
        {
            string searchValue = model.SearchValue;
            Guid categoryId = model.CategoryId.Value;

            Expression<Func<Post, bool>> predicate = p => p.Status == PostStatus.Published;
            if(!string.IsNullOrWhiteSpace(searchValue))
            {
                predicate = CombineAnd(predicate, p => p.Title.Contains(searchValue));
            }
            // Gộp điều kiện lọc theo CategoryId
            if (categoryId != Guid.Empty)
            {
                predicate = CombineAnd(predicate, p => p.CategoryId == categoryId);
            }
            // Include Category để lấy CategoryName
            var posts = _postRepository.FindAll(predicate, p => p.Category).Select(p => new PostViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                Content = p.Content,
                CoverImageUrl = p.ImageBase64,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                CategoryId = p.CategoryId.Value,
                Status = p.Status.ToString(),
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });



            var result = await posts.ToListAsync();
            return ResponseResult.Success(result);
        }
        public static Expression<Func<T, bool>> CombineAnd<T>(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    Expression.Invoke(expr1, parameter),
                    Expression.Invoke(expr2, parameter)
                ),
                parameter
            );
            return combined;
        }
        #endregion

        #region GET-BY-ID
        public async Task<ResponseResult<PostViewModel>> GetById(Guid id)
        {
            var post = await _postRepository.FindByIdAsync(id);
            if (post is null)
                return ResponseResult<PostViewModel>.NotFound();

            // Nạp Category (lazy loading hoặc truy vấn riêng)
            var categoryName = string.Empty;
            if (post.CategoryId.HasValue)
            {
                var category = await _categoryRepository.FindByIdAsync(post.CategoryId.Value);
                categoryName = category?.Name ?? string.Empty;
            }

            var result =  new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Content = post.Content,
                CoverImageUrl = post.ImageBase64,
                CategoryName = categoryName,
                CategoryId = post.CategoryId.Value,
                Status = post.Status.ToString(),
                PublishedAt = post.PublishedAt,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
            return ResponseResult.Success(result);
        }
        #endregion

        #region UPDATE
        public async Task<ResponseResult> Update(UpdatePostViewModel model)
        {
            var post = await _postRepository.FindByIdAsync(model.Id);
            if (post is null)
                return ResponseResult.NotFound();

            // Nếu CategoryId thay đổi thì kiểm tra hợp lệ
            if (model.CategoryId != Guid.Empty && model.CategoryId != post.CategoryId)
            {
                var categoryExist = await _categoryRepository.FindByIdAsync(model.CategoryId);
                if (categoryExist is null)
                    throw new Exception("Category not found");
                post.CategoryId = model.CategoryId;
            }
            if (!PostStatusHelper.TryParse(model.Status, out var postStatus))
            {
                throw new Exception($"Status “{model.Status}” không hợp lệ.");
            }
            var isPublished = false;
            if (postStatus == PostStatus.Published)
            {
                isPublished = true;
            }

            post.Title = model.Title?.Trim() ?? post.Title;
            post.Slug = model.Slug?.Trim() ?? post.Slug;
            post.Content = model.Content ?? post.Content;
            post.Excerpt = model.Excerpt ?? post.Excerpt;
            post.ImageBase64 = model.CoverImageUrl ?? post.ImageBase64;
            post.Status = postStatus;
            if(post.PublishedAt == null)
            {
                post.PublishedAt = isPublished ? DateTime.Now : null;
            }
            post.UpdatedAt = DateTime.UtcNow;

            _postRepository.Update(post);
            await _unitOfWork.SaveChange();
            return ResponseResult.Success("Update post successfully");
        }
        #endregion
    }
}
