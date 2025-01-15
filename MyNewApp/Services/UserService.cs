using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class UserService : IUserService
{
    public IUserRepository _userRepository { get; }

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool IsValidUser(User user)
    {
        var foundUser = _userRepository.FindByEmail(user.Email);

        if (foundUser is not null && user.Username == foundUser.Username && user.Password == foundUser.Password)
        {
            return true;
        }

        return false;
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

    public User AddUser(User user)
    {
        var newUser = _userRepository.Add(user);

        return newUser;
    }
}