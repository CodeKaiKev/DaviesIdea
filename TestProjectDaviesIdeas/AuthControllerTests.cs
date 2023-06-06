using DaviesIdeas.Models;

namespace TestProjectDaviesIdeas
{
    public class AuthControllerTests
    {
        [Fact]
        public void Register_ShouldReturnBadRequest_WhenUsernameAlreadyExists()
        {
            User user = new User
            {
                Username = "test1234",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
                Email = "test@test.com",
                Role = "Admin"
            };

            //When

            //Then
        }
    }
}