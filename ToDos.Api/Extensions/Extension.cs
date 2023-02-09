using AutoMapper;
using Gitos.Domain.Repository;
using MediatR;
using ToDos.Api.Commands;
using ToDos.Api.Middleware;
using ToDos.Api.Queries;
using ToDos.Infrastructure.Data.Cosmos;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Extensions
{
    public static class Extensions
    {
        public static void MapToDoRoutes(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/todos/");

            group.MapPost("create", async (IMediator mediator, IMapper mapper, CreateToDo todo) =>
            {
                return await mediator.Send(mapper.Map<SaveToDoCommand>(todo));
            });

            group.MapPut("update", async (IMediator mediator, IMapper mapper, ToDo todo) =>
            {
                return await mediator.Send(mapper.Map<SaveToDoCommand>(todo));
            });

            group.MapDelete("delete", async (IMediator mediator, IMapper mapper, Guid id) =>
            {
                return await mediator.Send(mapper.Map<DeleteToDoCommand>(id));
            });

            group.MapGet("", async (IMediator _mediator) =>
            {
                return await _mediator.Send(new GetToDosQuery());
            });

            group.MapGet("{id}", async (IMediator _mediator, IMapper mapper, Guid id) =>
            {
                return await _mediator.Send(mapper.Map<GetToDoByIdQuery>(id));
            });


        }

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

        public static void AddToDosMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
