using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PU.Users.Api.DTOs;
using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace PU.Users.Tests;

public class UsersCrudIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public UsersCrudIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_Update_Delete_User_Workflow()
    {
        // Create group to attach
        var groupsRes = await _client.GetAsync("/api/groups");
        groupsRes.EnsureSuccessStatusCode();
        var groups = await groupsRes.Content.ReadFromJsonAsync<List<GroupDto>>();
        int groupId = groups?.FirstOrDefault()?.Id ?? 0;

        var create = new { firstName = "Intg", lastName = "Test", email = "intg@example.com", groupIds = new int[] { groupId } };
        var createRes = await _client.PostAsJsonAsync("/api/users", create);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var created = await createRes.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(created);
        Assert.Equal("Intg", created.FirstName);

        // Update
        var update = new { firstName = "Intg2", lastName = "Test", email = "intg2@example.com", groupIds = new int[] { groupId } };
        var putRes = await _client.PutAsJsonAsync($"/api/users/{created.Id}", update);
        Assert.True(putRes.StatusCode == HttpStatusCode.NoContent || putRes.StatusCode == HttpStatusCode.OK);

        // Get and verify
        var getRes = await _client.GetAsync($"/api/users/{created.Id}");
        getRes.EnsureSuccessStatusCode();
        var got = await getRes.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal("Intg2", got.FirstName);

        // Delete
        var delRes = await _client.DeleteAsync($"/api/users/{created.Id}");
        Assert.True(delRes.StatusCode == HttpStatusCode.NoContent || delRes.StatusCode == HttpStatusCode.OK);

        // Verify deleted
        var afterGet = await _client.GetAsync($"/api/users/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterGet.StatusCode);
    }
}
