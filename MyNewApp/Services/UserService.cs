using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

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