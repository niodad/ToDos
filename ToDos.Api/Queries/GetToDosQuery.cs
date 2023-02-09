using MediatR;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Queries
{
    public class GetToDosQuery : IRequest<IEnumerable<ToDo>>
    {

    }
}

