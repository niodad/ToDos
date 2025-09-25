using ToDos.Domain.Interfaces;
using MediatR;
using ToDos.Api.Queries;
using ToDos.Domain.Exceptions;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Handlers
{
    public class GetToDoByIdQueryHandler : IRequestHandler<GetToDoByIdQuery, ToDo>
    {
        private readonly IRepository<ToDo, Guid> _repository;

        public GetToDoByIdQueryHandler(IRepository<ToDo, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<ToDo> Handle(GetToDoByIdQuery request, CancellationToken cancellationToken)
        {
            return (await _repository.GetAsync(i => i.Id == request.Id))
                .FirstOrDefault() ?? throw new NotFoundException();
        }
    }
}

