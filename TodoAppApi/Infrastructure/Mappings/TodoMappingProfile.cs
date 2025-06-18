using AutoMapper;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.Entities;

namespace TodoAppApi.Infrastructure.Mappings
{
    public class TodoMappingProfile : Profile
    {
        public TodoMappingProfile()
        {
            CreateMap<Todo, TodoResponseDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TodoTags.Select(tt => tt.Tag.Name)))
                .ReverseMap();

            CreateMap<TodoRequestDto, Todo>();
        }
    }
}
