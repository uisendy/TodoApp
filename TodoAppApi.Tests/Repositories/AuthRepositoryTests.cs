using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoAppApi.Tests.Repositories
{
    using System;
    using System.Threading.Tasks;
    using TodoAppApi.Entities;
    using TodoAppApi.Repositories;
    using TodoAppApi.Tests.Helpers;
    using Xunit;

    public class AuthRepositoryTests
    {
        [Fact]
        public async Task AddUserAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryContext();
            var repo = new AuthRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com",
                PasswordHash = "hashedpw"
            };

            // Act
            await repo.AddUserAsync(user);
            await repo.SaveChangesAsync();

            var result = await repo.GetUserByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }
    }

}
