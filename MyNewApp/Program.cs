using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Welcome to the web app!");

var todos = new List<TodoItem>
{
    // new TodoItem(1, "Learn C#", DateTime.Now.AddDays(1), false),
    // new TodoItem(2, "Build a web app", DateTime.Now.AddDays(2), false),
    // new TodoItem(3, "Publish to Azure", DateTime.Now.AddDays(3), false)
};

app.MapGet("/todos", () => todos);

app.MapGet("/todos/{id}", Results<Ok<TodoItem>, NotFound> (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);

    return todo is not null ? TypedResults.Ok(todo) : TypedResults.NotFound();
});

app.MapPost("/todos", Results<Created<TodoItem>, BadRequest> (TodoItem todo) =>
{
    if (string.IsNullOrWhiteSpace(todo.Title))
    {
        return TypedResults.BadRequest();
    }

    var nextId = todos.Any() ? todos.Max(t => t.Id) + 1 : 1;

    todo = todo with { Id = nextId };
    todos.Add(todo);

    return TypedResults.Created("/todos/{id}", todo);
})
.AddEndpointFilter(async (context, next) =>
{
    var todo = context.GetArgument<TodoItem>(0);
    var errors = new Dictionary<string, string[]>();

    if (todo.DueDate < DateTime.Now)
    {
        errors.Add(nameof(TodoItem.DueDate), ["Due date cannot be in the past."]);
    }

    if (todos.Any(t => t.Title == todo.Title))
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
});

app.MapDelete("/todos/{id}", Results<NoContent, NotFound> (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);

    if (todo is null)
    {
        return TypedResults.NotFound();
    }

    todos.Remove(todo);

    return TypedResults.NoContent();
});

app.Run();


public record TodoItem(int Id, string Title, DateTime DueDate, bool IsCompleted);