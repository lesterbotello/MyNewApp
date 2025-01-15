public interface IUserRepository
{
    User Add(User User);

    IEnumerable<User> GetAll();

    User FindByEmail(string email);

    User Update(string email, User user);

    void Delete(string email);

    IEnumerable<User> AddMany(IEnumerable<User> users);
}