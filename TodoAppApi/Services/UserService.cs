using TodoAppApi.DTOs.Users;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto> GetUserProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null
                ? throw new Exception("User not found")
                : new UserResponseDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.Phone ?? "" ,
                        Bio = user.Bio ?? ""
                    };
        }

        public async Task UpdateUserProfileAsync(Guid userId, UpdateUserRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new Exception("User not found");
            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            if (!string.IsNullOrWhiteSpace(dto.Bio))
                user.Bio = dto.Bio;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }

    }
}
