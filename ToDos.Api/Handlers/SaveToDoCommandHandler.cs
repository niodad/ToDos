using AutoMapper;
using Gitos.Domain.Repository;
using MediatR;
using ToDos.Api.Commands;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Handlers
{
    public class SaveToDoCommandHandler : IRequestHandler<SaveToDoCommand, ToDo>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<ToDo, Guid> _repository;

        public SaveToDoCommandHandler(IMapper mapper, IRepository<ToDo, Guid> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<ToDo> Handle(SaveToDoCommand request, CancellationToken cancellationToken)
        {
            return await _repository.SaveAsync(_mapper.Map<ToDo>(request));
        }
    }
}
