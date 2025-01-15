using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn), ServiceLifetime.Singleton);
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ITodoRepository, TodoRepository>();

builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// After adding openapi, you map it here:
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = string.Empty;
});

var todoRepository = app.Services.GetRequiredService<ITodoRepository>();

LoggingEndpoints.Map(app, app.Logger);

// Login endpoint
app.MapPost("/login", (User user, IUserService userService) => Login(user, userService));

IResult Login(User user, IUserService userService)
{
    if(!string.IsNullOrWhiteSpace(user.Username) && 
        !string.IsNullOrWhiteSpace(user.Password) && 
        userService.IsValidUser(user))
    {
        var tokenString = userService.GenerateToken(user, app);

        return Results.Ok(tokenString);
    }
    
    return Results.BadRequest();
}

app.MapGet("/todos", () => todoRepository.GetAll())
    .RequireAuthorization();

// If you want to return multiple types of results, you can change the NotFound 
// generic type parameter by IResult
app.MapGet("/todos/{id:int}", Results<Ok<TodoItem>, NotFound> (int id) =>
{
    var todo = todoRepository.FindById(id);

    return todo is not null ? TypedResults.Ok(todo) : TypedResults.NotFound();
}).RequireAuthorization();


app.MapGet("/todos/{text}", (string text) => todoRepository.FindByTitle(text))
    .RequireAuthorization();

app.MapPost("/todos", Results<Created<TodoItem>, BadRequest<string>> (TodoItem todo) =>
{
    try
    {
        var addedTodo = todoRepository.Add(todo);
        return TypedResults.Created("/todos/{id}", addedTodo);
    }
    catch(Exception ex)
    {
        return TypedResults.BadRequest(ex.Message);
    }
})
.AddEndpointFilter(async (context, next) =>
{
    var todo = context.GetArgument<TodoItem>(0);
    var errors = new Dictionary<string, string[]>();

    if (todo.DueDate < DateTime.Now)
    {
        errors.Add(nameof(TodoItem.DueDate), ["Due date cannot be in the past."]);
    }

    if (todoRepository.GetAll().Any(t => t.Title == todo.Title))
    {
        errors.Add(nameof(TodoItem.Title), ["Todo with the same title already exists."]);
    }

    if (todo.IsCompleted)
    {
        errors.Add(nameof(TodoItem.IsCompleted), ["Cannot add a completed todo."]);
    }

    if(errors.Any())
    {
        return Results.ValidationProblem(errors);
    }

    return await next(context);
}).RequireAuthorization();

app.MapPut("/todos/{id}", Results<Ok<TodoItem>, NotFound> (int id, TodoItem todo) =>
{
    var existingTodo = todoRepository.Update(id, todo);

    if (existingTodo is null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(existingTodo);
}).RequireAuthorization();

app.MapDelete("/todos/{id}", Results<NoContent, NotFound> (int id) =>
{
    try
    {
        todoRepository.Delete(id);
    }
    catch (Exception)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.NoContent();
}).AddEndpointFilter(async (context, next) => {
    var id = context.GetArgument<int>(0);
    var todo = todoRepository.GetAll().FirstOrDefault(todo => todo.Id == id);

    if (todo is null) 
    {
        return TypedResults.NotFound();
    }

    if(!todo.IsCompleted)
    {
        return TypedResults.BadRequest(new { Error = "Can't delete an uncompleted todo." });
    }

    return await next(context);
}).RequireAuthorization();

app.MapGet("/badendpoint", () => {
    throw new ApplicationException("This is a bad endpoint");
});

// Middleware to log the request execution time
// app.Use(async (context, next) =>
// {
//     var start = DateTime.UtcNow;
//     await next.Invoke(context);
//     app.Logger.LogInformation($"Request {context.Request.Method} {context.Request.Path} executed in {(DateTime.UtcNow - start).TotalMilliseconds}ms");
// });

// This is the refactored version of the middleware above
// app.UseMiddleware<PerformanceTimer>();

// This is the middleware above wrapped around an extension method
app.UsePerformanceTimer();

app.UseExceptionHandler(builder => {
    builder.Run(async context => {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        await context.Response.WriteAsync(JsonSerializer.Serialize(new { 
            context.Response.StatusCode,
            Error = "Internal Server Error. Contact support at support@abc.xyz" // We want to be as ambiguous as possible
        }));
    });
});

app.Run();