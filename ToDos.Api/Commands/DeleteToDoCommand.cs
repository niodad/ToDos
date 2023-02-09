using MediatR;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Commands
{
    public class DeleteToDoCommand : IRequest<ToDo>
    {
        public Guid Id { get; set; }
    }
}
