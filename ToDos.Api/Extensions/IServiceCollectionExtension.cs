using ToDos.Domain.Interfaces;
using MediatR;
using ToDos.Infrastructure.Data.Cosmos;
using ToDos.Api.Services;

namespace ToDos.Api.Extensions
{
    public static class IServiceCollectionExtension
    {

        public static void AddToDosServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(typeof(Program));
            services.AddAutoMapper(typeof(Program));
            services.AddScoped(typeof(IRepository<,>), typeof(CosmosDbRepository<,>));
            //use inmemory database
            //services.AddScoped(typeof(IRepository<,>), typeof(InmemoryRespository<,>));
            //services.AddDbContext<ToDosDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            services.AddDatabaseDeveloperPageExceptionFilter();
            
            // Add authentication services
            services.AddScoped<ApiKeyAuthenticationService>();
        }

    }
}
