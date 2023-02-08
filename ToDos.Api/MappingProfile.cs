using AutoMapper;
using ToDos.Api.Commands;
using ToDos.Infrastructure.Data;

namespace ToDos.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDo, SaveToDoCommand>().ReverseMap();
            CreateMap<Guid, DeleteToDoCommand>().ForMember(dest => dest.Id, opt => opt.MapFrom(s => s));

        }
    }
}
