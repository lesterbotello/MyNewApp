public interface IUserService 
{
    User Add(User user);

    bool IsValidUser(User user);

    string GenerateToken(User user, WebApplication app);
}