using Blog.Domain;
using Blog.Domain.Entities;
using Blog.Domain.Interface;
using Blog.Domain.ViewModel.Auth;
using Blog.Domain.ViewModel.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using StudentMngt.Domain.Interface;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Blog.Infrastructure.Repository
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User, Guid> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(
       IGenericRepository<User, Guid> userRepository,
       IUnitOfWork unitOfWork,
       IConfiguration configuration, ILogger<UserService> logger)                       
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<ResponseResult> Create(CreateUserViewModel model)
        {
            try
            {
                // Kiểm tra trùng username/email
                bool existed = await _userRepository.FindAll()
                                  .AnyAsync(u => u.Username == model.UserName ||
                                                 u.Email == model.Email);
                if (existed)
                    return ResponseResult.Fail("Username hoặc Email đã tồn tại.");

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    AvatarUrl = model.AvatarUrl,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                _userRepository.Add(user);
                await _unitOfWork.SaveChange();
                return ResponseResult.Success("Tạo user thành công");
            }
            catch (Exception ex)
            { 
                throw;             
            }
        }

        public async Task<ResponseResult> Delete(Guid id)
        {
            var user = await _userRepository.FindByIdAsync(id);
            if (user == null)
            {
                throw new Exception("Not found user by Id");
            }
            _userRepository.Remove(user);
            await _unitOfWork.SaveChange();
            return ResponseResult.Success("Delete category successfully");
        }

        public Task<ResponseResult> DeleteAll()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseResult<List<UserViewModel>>> GetAll()
        {
            var users =  _userRepository.FindAll().Select(u => new UserViewModel()
            {
                Id = u.Id,
                UserName = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Password = u.PasswordHash,
                AvatarUrl = u.AvatarUrl,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });
            var result = await users.ToListAsync();
       
            return ResponseResult.Success(result);
        }

        public async Task<ResponseResult<UserViewModel>> GetById(Guid id)
        {
            var user = await _userRepository.FindByIdAsync(id);
            if (user == null)
            {
                return ResponseResult<UserViewModel>.Fail("User not found", StatusCodes.Status404NotFound);
            }
            var result = new UserViewModel()
            {
                Id = id,
                UserName = user.Username,
                FullName = user.FullName,
                Password = user.PasswordHash,
                Email = user.Email,
                UpdatedAt = user.UpdatedAt,
                CreatedAt = user.CreatedAt,
                AvatarUrl = user.AvatarUrl
                
            };
            return ResponseResult.Success(result);
        }

        public async Task<ResponseResult> Update(UpdateUserViewModel model)
        {
            var user = await _userRepository.FindByIdAsync(model.Id);
            if (user is null)
                throw new Exception("User not found by Id");

            user.Username = model.UserName;
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.AvatarUrl = model.AvatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            _userRepository.Update(user);
            await _unitOfWork.SaveChange();
            return ResponseResult.Success("Cập nhật user thành công");
        }

        public async Task<ResponseResult> LoginAsync(LoginUserViewModel model)
        {
            var user = await _userRepository.FindAll().FirstOrDefaultAsync(u => u.Username == model.UserNameOrEmail
                                                                            || u.Email == model.UserNameOrEmail);
            if (user == null)
            {
                return ResponseResult.Fail("Account or Password incorrectly");
            }

            // So sánh mật khẩu
            var isValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!isValid)
            {
                return ResponseResult.Fail("Account or Password incorrectly");
            }

            _logger.LogDebug("Check username, password successfully");
            // Tạo Token
            var token = GenerateJwtToken(user);
            var expireAt = DateTime.UtcNow
              .AddMinutes(_configuration.GetValue<int>("Jwt:ExpiresInMinutes"));

            var data = new LoginResultViewModel
            {
                Token = token,
                ExpireAt = expireAt,
                UserName = user.Username,
                FullName = user.FullName
            };
            return ResponseResult.Success(data, "Login successfully");

        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("fullName", user.FullName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                               _configuration.GetValue<int>("Jwt:ExpiresInMinutes")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> FindUserById(Guid id)
        {
            var result = await _userRepository.FindByIdAsync(id);

            return result;
        }
    }
}
