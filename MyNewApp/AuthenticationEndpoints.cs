using Microsoft.AspNetCore.Http.HttpResults;

static class AuthenticationEndpoints
{
    public static void Map(WebApplication app, IUserService userService)
    {

         app.MapPost("/signup", Results<Created<User>, BadRequest<string>> (User user) =>
        {
              try
            {
                var addedUser = userService.Add(user);
                return TypedResults.Created("/signup/{id}", addedUser);
            }
            catch(Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        });


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