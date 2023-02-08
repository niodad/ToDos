using AutoMapper;
using ToDos.Api.Commands;
using ToDos.Api.DTO;
using ToDos.Api.Queries;
using ToDos.Infrastructure.Data;

namespace ToDos.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDo, SaveToDoCommand>().ReverseMap();
            CreateMap<CreateToDo, SaveToDoCommand>();
            CreateMap<Guid, DeleteToDoCommand>().ForMember(dest => dest.Id, opt => opt.MapFrom(s => s));
            CreateMap<Guid, GetToDoByIdQuery>().ForMember(dest => dest.Id, opt => opt.MapFrom(s => s));

        }
    }
}
