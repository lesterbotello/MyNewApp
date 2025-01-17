using AutoFixture;
using Moq;

[TestFixture]
public class UserServiceTests
{
    Fixture _fixture = new Fixture();

    [Test]
    public void User_IfUserValid_ShouldReturnTrue()
    {
        //var user = new User(1, "admin", "admin", "admin@admin.adm", "Admin", "Admin");

        var user = _fixture.Build<User>()
            .With(u => u.Username, "admin")
            .With(u => u.Password, "admin")
            .With(u => u.Email, "admin@admin.adm")
            .Create();

        // Arrange
        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(x => x.FindByEmail("admin@admin.adm")).Returns(user);
        var userService = new UserService(userRepository.Object);

        // Act
        var result = userService.IsValidUser(user);

        // Assert
        Assert.That(result, Is.True);
    }
}