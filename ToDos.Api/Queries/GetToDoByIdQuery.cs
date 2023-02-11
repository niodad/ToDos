using MediatR;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Queries
{
    public class GetToDoByIdQuery : IRequest<ToDo>
    {
        public GetToDoByIdQuery(Guid id, string email)
        {
            Id = id;
            Email = email;
        }

        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
    }
}

