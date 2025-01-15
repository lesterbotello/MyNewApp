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

        var nextId = _dbContext.Users.Any() ? _dbContext.Users.Max(t => t.Id) + 1 : 1;

        var newUser = new User(nextId, user.Username, user.Password, user.Email, user.GivenName, user.FamilyName);
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();

        return newUser;
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

    public User Update(string email, User user)
    {
        var existingUser = _dbContext.Users.FirstOrDefault(t => t.Email == email);

        if (existingUser is null)
        {
            throw new Exception("User not found.");
        }

        var existingId = existingUser.Id;

        var newUser = new User(existingId, user.Username, user.Password, email, user.GivenName, user.FamilyName);

        _dbContext.Users.Remove(existingUser);
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();

        return newUser;
    }
}