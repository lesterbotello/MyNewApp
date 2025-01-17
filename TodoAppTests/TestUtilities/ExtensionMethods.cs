using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

public static class HttpClientTestExtensions
{
    public static async Task SignInAsync(this HttpClient client)
    {
        var signInRespose = await client.PostAsJsonAsync("/login", 
            new { 
                Username = "admin", 
                Password = "admin", 
                Email = "admin@admin.adm" 
            }
        );
        var token = await signInRespose.Content.ReadAsStringAsync();
        token = token.Replace("\"", string.Empty);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}

public static class CustomWebApplicationFactoryTestExtensions
{
    public static void SeedDatabase(this CustomWebApplicationFactory<Program> application)
    {
        using(var scope = application.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TodoAppDbContext>();;

            Utilities.InitializeDbForTests(db);
        }
    }
}