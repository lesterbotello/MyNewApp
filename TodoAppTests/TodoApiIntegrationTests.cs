using System.Net;
using System.Net.Http.Json;

[TestFixture]
public class TodoApiIntegrationTests
{
    [Test]
    public async Task Todos_GetAll_ShouldReturnListOfTodos()
    {
        // Arrange
        await using var application = new CustomWebApplicationFactory<Program>();

        // Version without the extension method:
        // using(var scope = application.Services.CreateScope())
        // {
        //     var scopedServices = scope.ServiceProvider;
        //     var db = scopedServices.GetRequiredService<TodoAppDbContext>();;

        //     Utilities.InitializeDbForTests(db);
        // }

        application.SeedDatabase();
        var client = application.CreateClient();

        // Version without the extension method:
        // var signInRespose = await client.PostAsJsonAsync("/login", 
        //     new { 
        //         Username = "admin", 
        //         Password = "admin", 
        //         Email = "admin@admin.adm" 
        //     }
        // );
        // var token = await signInRespose.Content.ReadAsStringAsync();
        // token = token.Replace("\"", string.Empty);
        // client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await client.SignInAsync();

        // Act
        var response = await client.GetAsync("/todos");
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();

        // Assert
        Assert.That(todos, Is.TypeOf<List<Todo>>());
        Assert.That(todos.Count, Is.EqualTo(0)); // It's assumed the list of todos is empty
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Todos_GetById_ShouldReturnTodo()
    {
        // Arrange
        await using var application = new CustomWebApplicationFactory<Program>();
        application.SeedDatabase();
        var client = application.CreateClient();
        await client.SignInAsync();

        // Act
        await client.PostAsJsonAsync("/todos", new Todo(Id: 1, Title: "Test", DueDate: DateTime.Now.AddDays(1), IsCompleted: false));
        var response = await client.GetAsync("/todos/1");
        var todo = await response.Content.ReadFromJsonAsync<Todo>();

        // Assert
        Assert.That(todo, Is.TypeOf<Todo>());
        Assert.That(todo.Id, Is.EqualTo(1));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task Todos_GetById_ShouldReturnNotFound()
    {
        // Arrange
        await using var application = new CustomWebApplicationFactory<Program>();
        application.SeedDatabase();
        var client = application.CreateClient();
        await client.SignInAsync();

        // Act
        await client.PostAsJsonAsync("/todos", new Todo(Id: 1, Title: "Test", DueDate: DateTime.Now.AddDays(1), IsCompleted: false));
        var response = await client.GetAsync("/todos/5");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}