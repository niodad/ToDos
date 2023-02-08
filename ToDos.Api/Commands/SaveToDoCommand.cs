using MediatR;
using ToDos.Infrastructure.Data;

namespace ToDos.Api.Commands
{
    public class SaveToDoCommand : ToDo, IRequest<ToDo>
    {
    }
}
