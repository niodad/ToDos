using MediatR;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Queries
{
    public class GetToDosQuery : IRequest<IEnumerable<ToDo>>
    {
        public GetToDosQuery(string email)
        {
            Email = email;
        }

        public string Email { get; set; } = default!;
    }
}

