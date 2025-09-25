using AutoMapper;
using ToDos.Domain.Interfaces;
using MediatR;
using ToDos.Api.Commands;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Api.Handlers
{
    public class SaveToDoCommandHandler : IRequestHandler<SaveToDoCommand, ToDo>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<ToDo, Guid> _repository;
        private readonly ILogger<SaveToDoCommandHandler> _logger;

        public SaveToDoCommandHandler(IMapper mapper, IRepository<ToDo, Guid> repository, ILogger<SaveToDoCommandHandler> logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }
        
        public async Task<ToDo> Handle(SaveToDoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Saving ToDo with Name: {Name} and Email: {Email}", request.Name, request.Email);
                
                var todo = _mapper.Map<ToDo>(request);
                var result = await _repository.SaveAsync(todo);
                
                _logger.LogInformation("Successfully saved ToDo with Id: {Id}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving ToDo with Name: {Name} and Email: {Email}", request.Name, request.Email);
                throw;
            }
        }
    }
}
