using AutoMapper;
using MediatR;
using ToDos.Api.Commands;
using ToDos.Api.Queries;
using ToDos.Infrastructure.Data.Entities;
using ToDos.Api.Models;

namespace ToDos.Api.Extensions
{
    public static class IEndpointRouteBuilderExtension
    {
        public static void MapToDoRoutes(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/todos/");

            group.MapPost("", async (IMediator mediator, IMapper mapper, CreateToDo todo) =>
            {
                // Validation will be handled by ASP.NET Core model validation
                var result = await mediator.Send(mapper.Map<SaveToDoCommand>(todo));
                return Results.Created($"/api/todos/{result.Id}", result);
            })
            .WithName("CreateToDo")
            .WithOpenApi()
            .Produces<ToDo>(201)
            .Produces(400);

            group.MapPut("{id:guid}", async (IMediator mediator, IMapper mapper, Guid id, ToDo todo) =>
            {
                // Ensure the ID in the URL matches the entity ID
                if (todo.Id != Guid.Empty && todo.Id != id)
                {
                    return Results.BadRequest("ID in URL does not match ID in request body");
                }
                todo.Id = id; // Set the ID from the URL
                var result = await mediator.Send(mapper.Map<SaveToDoCommand>(todo));
                return Results.Ok(result);
            })
            .WithName("UpdateToDo")
            .WithOpenApi()
            .Produces<ToDo>(200)
            .Produces(400)
            .Produces(404);

            group.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
            {
                var result = await mediator.Send(new DeleteToDoCommand(id));
                return result != null ? Results.Ok(result) : Results.NotFound();
            })
            .WithName("DeleteToDo")
            .WithOpenApi()
            .Produces<ToDo>(200)
            .Produces(404);

            group.MapGet("user/{email}", async (IMediator _mediator, string email) =>
            {
                var result = await _mediator.Send(new GetToDosQuery(email));
                return Results.Ok(result);
            })
            .WithName("GetToDosByEmail")
            .WithOpenApi()
            .Produces<IEnumerable<ToDo>>(200)
            .Produces(400);

            group.MapGet("{id:guid}", async (IMediator _mediator, Guid id) =>
            {
                var result = await _mediator.Send(new GetToDoByIdQuery(id));
                return Results.Ok(result);
            })
            .WithName("GetToDoById")
            .WithOpenApi()
            .Produces<ToDo>(200)
            .Produces(404);


        }
    }
}
