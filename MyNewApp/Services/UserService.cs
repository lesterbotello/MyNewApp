using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class UserService : IUserService
{
    private readonly TodoAppDbContext _dbContext;

    public UserService(TodoAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
     public User Add(User user)
    {

        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.Email) ||  string.IsNullOrWhiteSpace(user.GivenName) ||  string.IsNullOrWhiteSpace(user.FamilyName))
        {
            throw new Exception("All fields are required.");
        }
        
        var nextId = _dbContext.Users.Any() ? _dbContext.Users.Max(t => t.Id) + 1 : 1;

        user = user with { Id = nextId };
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return user;
    }

    public bool IsValidUser(User user)
    {

         var existingUser = _dbContext.Users.FirstOrDefault(u => u.Username == user.Username);

        if (existingUser is null)
        {
            return false;
        }

        return true;
    }

    public string GenerateToken(User user, WebApplication app)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.GivenName),
            new Claim(ClaimTypes.Surname, user.FamilyName)
        };
        
        var token = new JwtSecurityToken(
            issuer: app.Configuration["Jwt:Issuer"],
            audience: app.Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(app.Configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}