using Microsoft.AspNetCore.Mvc.Testing;
using PU.Users.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

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
        // Get existing groups
        var groupsRes = await _client.GetAsync("/api/groups");
        groupsRes.EnsureSuccessStatusCode();
        var groups = await groupsRes.Content.ReadFromJsonAsync<List<GroupDto>>();

        // If no groups exist, create a test group
        if (groups == null || !groups.Any())
        {
            var createGroup = new { name = "Test Group" };
            var createRes = await _client.PostAsJsonAsync("/api/groups", createGroup);
            createRes.EnsureSuccessStatusCode();
            var createdGroup = await createRes.Content.ReadFromJsonAsync<GroupDto>();

            if (createdGroup is not null)
                groups = new List<GroupDto> { createdGroup };
            else
                groups = new List<GroupDto>();
        }

        // Safely get groupId
        int groupId = groups.FirstOrDefault()?.Id ?? 0;
        if (groupId == 0)
            throw new InvalidOperationException("No groups available for integration test.");


        // Create user
        var createUser = new
        {
            firstName = "Intg",
            lastName = "Test",
            email = "intg@example.com",
            groupIds = new int[] { groupId }
        };
        var createUserRes = await _client.PostAsJsonAsync("/api/users", createUser);
        Assert.Equal(HttpStatusCode.Created, createUserRes.StatusCode);

        var createdUser = await createUserRes.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(createdUser);
        Assert.Equal("Intg", createdUser.FirstName);

        // Update user
        var updateUser = new
        {
            firstName = "Intg2",
            lastName = "Test",
            email = "intg2@example.com",
            groupIds = new int[] { groupId }
        };
        var updateRes = await _client.PutAsJsonAsync($"/api/users/{createdUser.Id}", updateUser);
        Assert.True(updateRes.StatusCode == HttpStatusCode.NoContent || updateRes.StatusCode == HttpStatusCode.OK);

        // Get and verify updated user
        var getRes = await _client.GetAsync($"/api/users/{createdUser.Id}");
        getRes.EnsureSuccessStatusCode();
        var updatedUser = await getRes.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(updatedUser);
        Assert.Equal("Intg2", updatedUser.FirstName);

        // Delete user
        var deleteRes = await _client.DeleteAsync($"/api/users/{createdUser.Id}");
        Assert.True(deleteRes.StatusCode == HttpStatusCode.NoContent || deleteRes.StatusCode == HttpStatusCode.OK);

        // Verify deletion
        var afterDelete = await _client.GetAsync($"/api/users/{createdUser.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }
}

