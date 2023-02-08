using Gitos.Domain.Repository;
using MediatR;
using ToDos.Api.Queries;
using ToDos.Infrastructure.Data;

namespace ToDos.Api.Handlers
{
    public class GetToDosQueryHandler : IRequestHandler<GetToDosQuery, IEnumerable<ToDo>>
    {
        private readonly IRepository<ToDo, Guid> _repository;

        public GetToDosQueryHandler(IRepository<ToDo, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ToDo>> Handle(GetToDosQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAsync(i => true);
        }
    }
}

