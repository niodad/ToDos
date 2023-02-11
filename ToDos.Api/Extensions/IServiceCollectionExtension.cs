using Gitos.Domain.Repository;
using MediatR;
using ToDos.Infrastructure.Data.Cosmos;

namespace ToDos.Api.Extensions
{
    public static class IServiceCollectionExtension
    {

        public static void AddToDosServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Program));
            services.AddAutoMapper(typeof(Program));
            services.AddScoped(typeof(IRepository<,>), typeof(CosmosDbRepository<,>));
            //use inmemory database
            //services.AddScoped(typeof(IRepository<,>), typeof(InmemoryRespository<,>));
            //services.AddDbContext<ToDosDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            services.AddDatabaseDeveloperPageExceptionFilter();
        }

    }
}
