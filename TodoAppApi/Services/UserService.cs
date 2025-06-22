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

        public async Task<UserResponseDto> UpdateUserProfileAsync(Guid userId, UpdateUserRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new Exception("User not found");

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ArgumentException("First name is required");
            user.FirstName = dto.FirstName ?? user.FirstName;

            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("Last name is required");
            user.LastName = dto.LastName ?? user.LastName;

            user.Phone = dto.Phone ?? user.Phone;
            user.Bio = dto.Bio ?? user.Bio;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone ?? "",
                Bio = user.Bio ?? ""
            };
        }


    }
}
