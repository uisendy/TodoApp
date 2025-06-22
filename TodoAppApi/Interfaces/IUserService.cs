using TodoAppApi.DTOs.Users;
using TodoAppApi.Entities;

namespace TodoAppApi.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserProfileAsync(Guid userId);
        Task UpdateUserProfileAsync(Guid userId, UpdateUserRequestDto dto);
    }
}