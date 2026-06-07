using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SCA.IntegrationTests.Features.Chat;

/// <summary>
/// Basic API integration tests for the interview demo.
/// </summary>
public sealed class ChatEndpointTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    /// <summary>
    /// Valid chat request returns the standard response shape.
    /// </summary>
    [Fact]
    public async Task Chat_WithValidRequest_ReturnsStandardResponse()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.PostAsync("/api/v1/chat", JsonContent("{\"message\":\"Merhaba\"}"));
        string json = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(ReadBoolean(json, "success"));
        Assert.Contains("Test cevabı: Merhaba", json);
    }

    /// <summary>
    /// The same question should be answered from cache after the first call.
    /// </summary>
    [Fact]
    public async Task Chat_WithSameQuestion_UsesCache()
    {
        HttpClient client = CreateClient();
        int before = factory.OllamaClient.CallCount;

        await client.PostAsync("/api/v1/chat", JsonContent("{\"message\":\"Aynı soru\"}"));
        await client.PostAsync("/api/v1/chat", JsonContent("{\"message\":\"Aynı soru\"}"));

        Assert.Equal(before + 1, factory.OllamaClient.CallCount);
    }

    /// <summary>
    /// Empty message should fail with the same response shape.
    /// </summary>
    [Fact]
    public async Task Chat_WithInvalidRequest_Returns400()
    {
        HttpClient client = CreateClient();

        HttpResponseMessage response = await client.PostAsync("/api/v1/chat", JsonContent("{\"message\":\"\"}"));
        string json = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(ReadBoolean(json, "success"));
    }

    /// <summary>
    /// Missing authentication should return the standard response shape.
    /// </summary>
    [Fact]
    public async Task Chat_WithoutAuthentication_Returns401WithStandardResponse()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.PostAsync("/api/v1/chat", JsonContent("{\"message\":\"Merhaba\"}"));
        string json = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(ReadBoolean(json, "success"));
    }

    /// <summary>
    /// Health endpoint should work without authentication.
    /// </summary>
    [Fact]
    public async Task Health_ReturnsOkWithoutAuthentication()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/v1/health");
        string json = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(ReadBoolean(json, "success"));
    }

    private HttpClient CreateClient()
    {
        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String("admin:admin123"u8.ToArray()));

        return client;
    }

    private static StringContent JsonContent(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static bool ReadBoolean(string json, string propertyName)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.GetProperty(propertyName).GetBoolean();
    }
}
