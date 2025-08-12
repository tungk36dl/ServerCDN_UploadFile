using Blog.Domain.Entities;
using Blog.Domain.ViewModel.Post;
using StudentMngt.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Interface
{
    public interface IPostService 
    {
        Task<ResponseResult<List<PostViewModel>>> GetAll();
        Task<ResponseResult<List<PostViewModel>>> GetPostPublished();
        Task<ResponseResult<List<PostViewModel>>> GetPostPublished(FilterPostModel model);
        //Task<ResponseResult<List<PostViewModel>>> SearchPost();
        Task<ResponseResult<PostViewModel>> GetById(Guid id);
        Task<ResponseResult> Create(CreatePostViewModel model);
        Task<ResponseResult> Update(UpdatePostViewModel model);
        Task<ResponseResult> Delete(Guid id);

    }
}
