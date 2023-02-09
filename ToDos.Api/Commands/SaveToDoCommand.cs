using MediatR;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Commands
{
    public class SaveToDoCommand : ToDo, IRequest<Infrastructure.Data.Entities.ToDo>
    {

    }
}
