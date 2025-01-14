using Microsoft.AspNetCore.Http.HttpResults;

static class TodoEndpoints
{
    public static void Map(WebApplication app, ITodoRepository todoRepository)
    {
        app.MapGet("/", () => "Welcome to the web app!");

        app.MapGet("/todos/{id:int}", Results<Ok<Todo>, NotFound> (int id) =>
        {
            var todo = todoRepository.FindById(id);

            return todo is not null ? TypedResults.Ok(todo) : TypedResults.NotFound();
        }).RequireAuthorization();

        app.MapGet("/todos", () => todoRepository.GetAll());

        app.MapGet("/todos/{text}", (string text) => todoRepository.FindByTitle(text));

        app.MapPost("/todos", Results<Created<Todo>, BadRequest<string>> (Todo todo) =>
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
            var todo = context.GetArgument<Todo>(0);
            var errors = new Dictionary<string, string[]>();

            if (todo.DueDate < DateTime.Now)
            {
                errors.Add(nameof(Todo.DueDate), ["Due date cannot be in the past."]);
            }

            if (todoRepository.GetAll().Any(t => t.Title == todo.Title))
            {
                errors.Add(nameof(Todo.Title), ["Todo with the same title already exists."]);
            }

            if (todo.IsCompleted)
            {
                errors.Add(nameof(Todo.IsCompleted), ["Cannot add a completed todo."]);
            }

            if(errors.Any())
            {
                return Results.ValidationProblem(errors);
            }

            return await next(context);
        });

        app.MapPut("/todos/{id}", Results<Ok<Todo>, NotFound> (int id, Todo todo) =>
        {
            var existingTodo = todoRepository.Update(id, todo);

            if (existingTodo is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(existingTodo);
        });

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
    }
}