public interface IUserService 
{
    bool IsValidUser(User user);

    string GenerateToken(User user, WebApplication app);
}