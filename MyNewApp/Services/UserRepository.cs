class UserRepository : IUserRepository
{
    private readonly TodoAppDbContext _dbContext;

    public UserRepository(TodoAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User Add(User user)
    {
        if (string.IsNullOrWhiteSpace(user.GivenName) || string.IsNullOrWhiteSpace(user.FamilyName) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
        {
            throw new Exception("User information is missing.");
        }

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return user;
    }

    public IEnumerable<User> AddMany(IEnumerable<User> Users)
    {
        List<User> addedUsers = new();

        foreach (var User in Users)
        {
            Add(User);
            addedUsers.Add(User);
        }

        return addedUsers;
    }

    public void Delete(string email)
    {
        var User = _dbContext.Users.FirstOrDefault(t => t.Email == email);

        if (User is null)
        {
            throw new Exception("User not found.");
        }

        _dbContext.Users.Remove(User);
        _dbContext.SaveChanges();
    }

    public User FindByEmail(string email)
    {
        return _dbContext.Users?.FirstOrDefault(t => t.Email == email);
    }

    public IEnumerable<User> GetAll() => _dbContext.Users;

    public User Update(string email, User User)
    {
        var existingUser = _dbContext.Users.FirstOrDefault(t => t.Email == email);

        if (existingUser is null)
        {
            throw new Exception("User not found.");
        }

        _dbContext.Users.Remove(existingUser);
        var newUser = User with { Email = email };
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();

        return newUser;
    }
}