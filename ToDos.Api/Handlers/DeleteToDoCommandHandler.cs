using AutoMapper;
using ToDos.Domain.Interfaces;
using MediatR;
using ToDos.Api.Commands;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Handlers
{
    public class DeleteToDoCommandHandler : IRequestHandler<DeleteToDoCommand, ToDo>
    {
        private readonly IRepository<ToDo, Guid> _repository;

        public DeleteToDoCommandHandler(IRepository<ToDo, Guid> repository, IMapper mapper)
        {
            _repository = repository;
        }
        public async Task<ToDo> Handle(DeleteToDoCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteAsync(request.Id);
        }
    }
}
