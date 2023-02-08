using AutoMapper;
using MediatR;
using System;
using ToDos.Api.Commands;
using ToDos.Api.Queries;
using ToDos.Infrastructure.Data;

namespace ToDos.Api.Extensions
{
    public static class Extensions
    {
        public static void MapToDoRoutes(this IEndpointRouteBuilder app)
        {
            app.MapPost("todos/create", async (IMediator mediator, IMapper mapper, ToDo todo) =>
            {
                return await mediator.Send(mapper.Map<SaveToDoCommand>(todo));
            });

            app.MapPut("todos/update", async (IMediator mediator, IMapper mapper, ToDo todo) =>
            {
                return await mediator.Send(mapper.Map<SaveToDoCommand>(todo));
            });

            app.MapDelete("todos/delete", async (IMediator mediator, IMapper mapper, Guid id) =>
            {
                return await mediator.Send(mapper.Map<DeleteToDoCommand>(id));
            });

            app.MapGet("todos", async (IMediator _mediator) =>
            {
                return await _mediator.Send(new GetToDosQuery());
            });

        }
    }
}
