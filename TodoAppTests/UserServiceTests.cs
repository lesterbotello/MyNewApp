using AutoFixture;
using Moq;

[TestFixture]
public class UserServiceTests
{
    Fixture _fixture = new Fixture();

    [Test]
    public void User_IfIsValid_ShouldReturnTrue()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(u => u.Username, "admin")
            .With(u => u.Password, "admin")
            .With(u => u.Email, "admin@admin.adm")
            .Create();

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(userRepository => userRepository.FindByEmail("admin@admin.adm")).Returns(user);
        var sut = new UserService(userRepositoryMock.Object);

        // Act
        var result = sut.IsValidUser(user);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void User_IfIsNotValid_ShouldReturnFalse()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(u => u.Username, "admin")
            .With(u => u.Password, "admin")
            .With(u => u.Email, "pepito@escuelota.com")
            .Create();

        var returnedUser = _fixture.Create<User>();

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(userRepository => userRepository.FindByEmail("pepito@escuelota.com")).Returns(returnedUser);
        var sut = new UserService(userRepositoryMock.Object);

        // Act
        var result = sut.IsValidUser(user);

        // Assert
        Assert.That(result, Is.False);
    }
}