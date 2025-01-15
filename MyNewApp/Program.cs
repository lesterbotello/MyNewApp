using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TodoAppDbContext>(options => options.UseSqlite(conn), ServiceLifetime.Singleton);
builder.Services.AddSingleton<ITodoRepository, TodoRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

var todoRepository = app.Services.GetRequiredService<ITodoRepository>();
var userService = app.Services.GetRequiredService<IUserService>();

TodoEndpoints.Map(app, todoRepository);

AuthenticationEndpoints.Map(app, userService);

// app.Use(async (ctx, next) => 
// {
//     var start = DateTime.Now;
//     await next.Invoke();
//     app.Logger.LogInformation($"Request duration: {(DateTime.Now - start).TotalMilliseconds}");
//     await next();
// });

app.Run();