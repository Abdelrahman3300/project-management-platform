
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FullStackProject.API.Tests;

public class ApiIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("FullStackProject API is running", content);
    }

    [Fact]
    public async Task GetProjects_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/Projects");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetProjects_WithInvalidToken_ReturnsUnauthorized()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/Projects");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "invalid.token.value");

        var response = await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task RegisterAndCreateProject_WithValidToken_ReturnsCreated()
    {
        // Arrange: create a unique test user.
        var email = $"integration-{Guid.NewGuid():N}@example.com";

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                firstName = "Integration",
                lastName = "Tester",
                email,
                password = "TestPassword123!"
            });

        Assert.Equal(
            HttpStatusCode.OK,
            registerResponse.StatusCode);

        var authJson = await registerResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var token = authJson.GetProperty("token").GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        // Act: request the authenticated projects endpoint.
        using var getProjectsRequest = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/Projects");

        getProjectsRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var getProjectsResponse =
            await _client.SendAsync(getProjectsRequest);

        // Assert: authenticated access succeeds.
        Assert.Equal(
            HttpStatusCode.OK,
            getProjectsResponse.StatusCode);

        // Act: create a project using the same JWT.
        using var createProjectRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/Projects");

        createProjectRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        createProjectRequest.Content = JsonContent.Create(new
        {
            name = "Integration Test Project",
            description = "Created by an API integration test."
        });

        var createProjectResponse =
            await _client.SendAsync(createProjectRequest);

        // Assert: project creation succeeds.
        Assert.Equal(
            HttpStatusCode.Created,
            createProjectResponse.StatusCode);

        var projectJson = await createProjectResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Integration Test Project",
            projectJson.GetProperty("name").GetString());

        Assert.True(
            projectJson.GetProperty("id").GetGuid() != Guid.Empty);
    }

    [Fact]
    public async Task ProjectEndpoints_NonOwner_ReturnsExpectedStatusCodes()
    {
        // Arrange: register two independent users.
        var ownerToken = await RegisterAndGetTokenAsync(
            "Project", "Owner");

        var otherUserToken = await RegisterAndGetTokenAsync(
            "Other", "User");

        // Create a project as the owner.
        using var createRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/Projects");

        createRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        createRequest.Content = JsonContent.Create(new
        {
            name = "Authorization Test Project",
            description = "Testing project ownership."
        });

        var createResponse = await _client.SendAsync(createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Get the project ID from the Location response header.
        var location = createResponse.Headers.Location;

        Assert.NotNull(location);

        var projectId = Guid.Parse(
            location!.Segments.Last().Trim('/'));

        // Act 1: another user tries to read the project.
        using var getRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/Projects/{projectId}");

        getRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        var getResponse = await _client.SendAsync(getRequest);

        // Assert 1: inaccessible projects return 404.
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Act 2: another user tries to update the project.
        using var updateRequest = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/Projects/{projectId}");

        updateRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        updateRequest.Content = JsonContent.Create(new
        {
            name = "Unauthorized Update",
            description = "This update should be rejected."
        });

        var updateResponse = await _client.SendAsync(updateRequest);

        // Assert 2: only the owner can update.
        Assert.Equal(HttpStatusCode.Forbidden, updateResponse.StatusCode);

        // Act 3: another user tries to delete the project.
        using var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/Projects/{projectId}");

        deleteRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        var deleteResponse = await _client.SendAsync(deleteRequest);

        // Assert 3: only the owner can delete.
        Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
    }

    private async Task<string> RegisterAndGetTokenAsync(
    string firstName,
    string lastName)
    {
        var email = $"integration-{Guid.NewGuid():N}@example.com";

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                firstName,
                lastName,
                email,
                password = "TestPassword123!"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authJson = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        var token = authJson.GetProperty("token").GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        return token!;
    }

    [Fact]
    public async Task CreateProject_WithMissingName_ReturnsBadRequest()
    {
        var token = await RegisterAndGetTokenAsync("Validation", "Tester");

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/Projects");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(new
        {
            description = "A project without a name."
        });

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task TaskEndpoints_OwnerCanManageTasks_NonMemberCannotReadTask()
    {
        var ownerToken = await RegisterAndGetTokenAsync("Task", "Owner");
        var otherUserToken = await RegisterAndGetTokenAsync("Other", "User");

        var projectId = await CreateProjectAsync(ownerToken);

        // Create task.
        using var createTaskRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/projects/{projectId}/tasks");

        createTaskRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        createTaskRequest.Content = JsonContent.Create(new
        {
            title = "Integration Test Task",
            description = "Testing task creation."
        });

        var createTaskResponse =
            await _client.SendAsync(createTaskRequest);

        Assert.Equal(HttpStatusCode.Created, createTaskResponse.StatusCode);

        var taskJson = await createTaskResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        var taskId = taskJson.GetProperty("id").GetGuid();

        Assert.NotEqual(Guid.Empty, taskId);

        // Get task list.
        using var getTasksRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/projects/{projectId}/tasks");

        getTasksRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        var getTasksResponse =
            await _client.SendAsync(getTasksRequest);

        Assert.Equal(HttpStatusCode.OK, getTasksResponse.StatusCode);

        var tasks = await getTasksResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(JsonValueKind.Array, tasks.ValueKind);
        Assert.True(tasks.GetArrayLength() >= 1);

        // Get individual task.
        using var getTaskRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/projects/{projectId}/tasks/{taskId}");

        getTaskRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        var getTaskResponse =
            await _client.SendAsync(getTaskRequest);

        Assert.Equal(HttpStatusCode.OK, getTaskResponse.StatusCode);

        var individualTask = await getTaskResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(taskId, individualTask.GetProperty("id").GetGuid());

        // Another user cannot retrieve the task.
        using var unauthorizedRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/projects/{projectId}/tasks/{taskId}");

        unauthorizedRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", otherUserToken);

        var unauthorizedResponse =
            await _client.SendAsync(unauthorizedRequest);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            unauthorizedResponse.StatusCode);
    }

    [Fact]
    public async Task CreateTask_AssignedToNonMember_ReturnsBadRequest()
    {
        var ownerToken = await RegisterAndGetTokenAsync(
            "Assignment", "Owner");

        var nonMember = await RegisterAndGetUserAsync(
            "External", "User");

        var projectId = await CreateProjectAsync(ownerToken);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/projects/{projectId}/tasks");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        request.Content = JsonContent.Create(new
        {
            title = "Invalid Assignment Task",
            description = "The assigned user is not a project member.",
            assignedToUserId = nonMember.UserId
        });

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ProjectMembers_NonOwnerCannotReadAddOrRemoveMembers()
    {
        var ownerToken = await RegisterAndGetTokenAsync(
            "Member", "Owner");

        var member = await RegisterAndGetUserAsync(
            "Project", "Member");

        var outsider = await RegisterAndGetUserAsync(
            "Outside", "User");

        var projectId = await CreateProjectAsync(ownerToken);

        // Owner adds a member.
        using var addMemberRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/projects/{projectId}/members");

        addMemberRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        addMemberRequest.Content = JsonContent.Create(new
        {
            userId = member.UserId,
            role = "Member"
        });

        var addMemberResponse =
            await _client.SendAsync(addMemberRequest);

        Assert.Equal(HttpStatusCode.OK, addMemberResponse.StatusCode);

        // Owner can retrieve members.
        using var getMembersRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/projects/{projectId}/members");

        getMembersRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        var getMembersResponse =
            await _client.SendAsync(getMembersRequest);

        Assert.Equal(HttpStatusCode.OK, getMembersResponse.StatusCode);

        // Outsider cannot retrieve members.
        using var outsiderGetRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/projects/{projectId}/members");

        outsiderGetRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", outsider.Token);

        var outsiderGetResponse =
            await _client.SendAsync(outsiderGetRequest);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            outsiderGetResponse.StatusCode);

        // Outsider cannot add a member.
        using var outsiderAddRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/projects/{projectId}/members");

        outsiderAddRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", outsider.Token);

        outsiderAddRequest.Content = JsonContent.Create(new
        {
            userId = outsider.UserId,
            role = "Member"
        });

        var outsiderAddResponse =
            await _client.SendAsync(outsiderAddRequest);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            outsiderAddResponse.StatusCode);

        // Outsider cannot remove a member.
        using var outsiderRemoveRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/projects/{projectId}/members/{member.UserId}");

        outsiderRemoveRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", outsider.Token);

        var outsiderRemoveResponse =
            await _client.SendAsync(outsiderRemoveRequest);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            outsiderRemoveResponse.StatusCode);

        // Owner can remove the member.
        using var ownerRemoveRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/projects/{projectId}/members/{member.UserId}");

        ownerRemoveRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", ownerToken);

        var ownerRemoveResponse =
            await _client.SendAsync(ownerRemoveRequest);

        Assert.Equal(
            HttpStatusCode.NoContent,
            ownerRemoveResponse.StatusCode);
    }

    private async Task<(string Token, Guid UserId)>
    RegisterAndGetUserAsync(string firstName, string lastName)
    {
        var email = $"integration-{Guid.NewGuid():N}@example.com";

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                firstName,
                lastName,
                email,
                password = "TestPassword123!"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authJson = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        var token = authJson.GetProperty("token").GetString();
        var userId = authJson.GetProperty("userId").GetGuid();

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.NotEqual(Guid.Empty, userId);

        return (token!, userId);
    }

    private async Task<Guid> CreateProjectAsync(string token)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/Projects");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(new
        {
            name = $"Integration Project {Guid.NewGuid():N}",
            description = "Created for integration testing."
        });

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var projectJson = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        var projectId = projectJson.GetProperty("id").GetGuid();

        Assert.NotEqual(Guid.Empty, projectId);

        return projectId;
    }
}
