using Blog.Infrastructure;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Đọc thông số từ appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");

// 2️⃣ Đăng ký AUTHENTICATION
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,

//            ValidIssuer = jwtSettings["Issuer"],
//            ValidAudience = jwtSettings["Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(
//                                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
//            ClockSkew = TimeSpan.Zero              // Không “nới” 5 phút mặc định
//        };
//    });


// 3️⃣ Đăng ký AUTHORIZATION (policy, role … nếu cần)
//builder.Services.AddAuthorization();

// cho phép yêu cầu từ frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Cấu hình đúng URL frontend
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .SetIsOriginAllowed(origin => true) // Chấp nhận tất cả origin
                  .WithExposedHeaders("*"); // Cho phép tất cả headers phản hồi
        });
});

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(); // Cho phép truy cập file tĩnh trong wwwroot

app.UseCors("AllowAllOrigins");          // (1)  CORS

app.UseAuthentication();                 // (2)  XÁC THỰC  ➜ gắn User principal

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
