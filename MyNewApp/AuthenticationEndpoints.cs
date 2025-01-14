using Microsoft.AspNetCore.Http.HttpResults;

static class AuthenticationEndpoints
{
    public static void Map(WebApplication app, IUserService userService)
    {
        app.MapPost("/login", Results<Ok<string>, BadRequest<string>> (User user) =>
        {
            if (userService.IsValidUser(user))
            {
                return TypedResults.Ok(userService.GenerateToken(user, app));
            }

            return TypedResults.BadRequest("Usuario o contraseña inválidos.");
        });
    }
}