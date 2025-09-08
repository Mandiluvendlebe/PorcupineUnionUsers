using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PU.Users.Api.Data;
using PU.Users.Api.Models;
using System.Linq;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            if (env.EnvironmentName == "Test")
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // ✅ Seed a default group for integration tests
                if (!db.Groups.Any())
                {
                    db.Groups.Add(new Group
                    {
                        Name = "Integration Test Group"
                    });
                    db.SaveChanges();
                }
            }
        });
    }
}
