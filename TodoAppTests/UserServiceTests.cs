using Moq;

[TestFixture]
public class UserServiceTests
{
    [Test]
    public void User_IfIsValid_ShouldReturnTrue()
    {
        // Arrange
        var user = new User(1, "admin", "admin", "admin@admin.adm", "Admin", "Admin");
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(userRepository => userRepository.FindByEmail("admin@admin.adm")).Returns(user);
        var sut = new UserService(userRepositoryMock.Object);

        // Act
        var result = sut.IsValidUser(user);

        // Assert
        Assert.That(result, Is.True);
    }
}