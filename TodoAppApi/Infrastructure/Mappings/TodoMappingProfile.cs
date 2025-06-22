using AutoMapper;
using TodoAppApi.DTOs.Todos;
using TodoAppApi.Entities;

namespace TodoAppApi.Infrastructure.Mappings
{
    public class TodoMappingProfile : Profile
    {
        public TodoMappingProfile()
        {
            CreateMap<TodoRequestDto, Todo>()
            .ForMember(dest => dest.TodoTags, opt => opt.Ignore());

            CreateMap<Todo, TodoResponseDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    src.TodoTags
                    .Where(tt => tt.Tag != null)
                    .Select(tt => new TodoTagDto
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name
                    }).ToList()
                ));
        }
    }
}