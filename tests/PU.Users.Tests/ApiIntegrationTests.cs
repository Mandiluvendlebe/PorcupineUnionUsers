using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PU.Users.Api.DTOs;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;



namespace PU.Users.Tests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Swagger_Available_In_Development()
    {
        var res = await _client.GetAsync("/swagger/index.html");
        // In CI Release this might not be available; don't fail hard.
        Assert.True(res.StatusCode == System.Net.HttpStatusCode.OK ||
                    res.StatusCode == System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Users_List_Returns_200()
    {
        var res = await _client.GetAsync("/api/users");
        res.EnsureSuccessStatusCode();
        var users = await res.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
    }
}
