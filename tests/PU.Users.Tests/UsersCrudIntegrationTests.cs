using Microsoft.AspNetCore.Mvc.Testing;
using PU.Users.Api.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PU.Users.Tests
{
    public class UsersCrudIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UsersCrudIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Users_CRUD_Workflow_Should_Work_Correctly()
        {
            // STEP 1: Get existing groups
            var groupsRes = await _client.GetAsync("/api/groups");

            // Allow empty as CI DB might have no groups yet
            if (groupsRes.StatusCode == HttpStatusCode.NotFound)
                groupsRes = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new List<GroupDto>())
                };

            groupsRes.EnsureSuccessStatusCode();

            var groups = await groupsRes.Content.ReadFromJsonAsync<List<GroupDto>>() ?? new List<GroupDto>();

            // STEP 2: Create a test group if none exist
            if (!groups.Any())
            {
                var createGroupPayload = new { name = "Integration Test Group" };
                var createGroupRes = await _client.PostAsJsonAsync("/api/groups", createGroupPayload);
                createGroupRes.EnsureSuccessStatusCode();

                var createdGroup = await createGroupRes.Content.ReadFromJsonAsync<GroupDto>();
                if (createdGroup != null)
                    groups.Add(createdGroup);
            }

            int groupId = groups.First().Id;

            // STEP 3: Create a user
            var createUserPayload = new
            {
                firstName = "Intg",
                lastName = "Test",
                email = "intg@example.com",
                groupIds = new[] { groupId }
            };

            var createUserRes = await _client.PostAsJsonAsync("/api/users", createUserPayload);
            Assert.Equal(HttpStatusCode.Created, createUserRes.StatusCode);

            var createdUser = await createUserRes.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(createdUser);
            Assert.Equal("Intg", createdUser!.FirstName);

            // STEP 4: Update the user
            var updateUserPayload = new
            {
                firstName = "Intg2",
                lastName = "Test",
                email = "intg2@example.com",
                groupIds = new[] { groupId }
            };

            var updateRes = await _client.PutAsJsonAsync($"/api/users/{createdUser.Id}", updateUserPayload);
            Assert.Contains(updateRes.StatusCode, new[] { HttpStatusCode.NoContent, HttpStatusCode.OK });

            // STEP 5: Get the updated user and verify
            var getRes = await _client.GetAsync($"/api/users/{createdUser.Id}");
            getRes.EnsureSuccessStatusCode();

            var updatedUser = await getRes.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(updatedUser);
            Assert.Equal("Intg2", updatedUser!.FirstName);

            // STEP 6: Delete the user
            var deleteRes = await _client.DeleteAsync($"/api/users/{createdUser.Id}");
            Assert.Contains(deleteRes.StatusCode, new[] { HttpStatusCode.NoContent, HttpStatusCode.OK });

            // STEP 7: Verify deletion
            var afterDeleteRes = await _client.GetAsync($"/api/users/{createdUser.Id}");
            Assert.Equal(HttpStatusCode.NotFound, afterDeleteRes.StatusCode);
        }
    }
}

