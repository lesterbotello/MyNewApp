public class UserService : IUserService
{
    public bool IsValidUser(User user)
    {
        if (user.Username == "admin" && user.Password == "admin")
        {
            return true;
        }

        return false;
    }
}