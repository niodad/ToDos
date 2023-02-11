using AutoMapper;
using ToDos.Api.Commands;
using ToDos.Infrastructure.Data.Entities;


namespace ToDos.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDo, SaveToDoCommand>().ReverseMap();
            CreateMap<CreateToDo, SaveToDoCommand>();
        }
    }
}
