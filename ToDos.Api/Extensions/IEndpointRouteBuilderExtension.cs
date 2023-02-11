using AutoMapper;
using MediatR;
using ToDos.Api.Commands;
using ToDos.Api.Queries;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Extensions
{
    public static class IEndpointRouteBuilderExtension
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
                return await mediator.Send(new DeleteToDoCommand(id));
            });

            group.MapGet("{email}", async (IMediator _mediator, string email) =>
            {
                return await _mediator.Send(new GetToDosQuery(email));
            });

            group.MapGet("{email}/{id}", async (IMediator _mediator, IMapper mapper, string email, Guid id) =>
            {
                return await _mediator.Send(new GetToDoByIdQuery(id, email));
            });


        }
    }
}
