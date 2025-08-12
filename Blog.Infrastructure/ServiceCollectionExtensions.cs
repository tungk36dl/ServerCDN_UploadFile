using Blog.Domain.Interface;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentMngt.Domain.Interface;


namespace Blog.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPostService, PostService>();  
            services.AddScoped<IUserService, UserService>();  

        }


        public static void AddRepositoryUOW(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        }


        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDBContext>(options => 
            options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));
        }

        //public static void AddRedisInfrastructure(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddStackExchangeRedisCache(redisOptions =>
        //    {
        //        var connectionString = configuration["ConnectionStrings:Redis"];
        //        redisOptions.Configuration = connectionString;
        //    });
        //}

    }
}
