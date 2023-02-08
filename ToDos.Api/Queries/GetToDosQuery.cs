using MediatR;
using ToDos.Infrastructure.Data;

namespace ToDos.Api.Queries
{
    public class GetToDosQuery : IRequest<IEnumerable<ToDo>>
    {
    }
}
