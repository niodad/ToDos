using MediatR;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Queries
{
    public class GetToDoByIdQuery : IRequest<ToDo>
    {
        public Guid Id { get; set; }

    }
}

