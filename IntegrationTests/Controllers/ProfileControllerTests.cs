using Common.Models;
using Infrastructure.Models;
using Infrastructure.Models.Users;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace IntegrationTests.Controllers;

public class ProfileControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    public ProfileControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri(TestConstants.ApiUrl)
        });
        _output = output;
    }

    [Fact]
    public async Task Explore_ReturnsCorrectStatusCode()
    {
        // Arrange
        var request = new ExploreRequest
        {
            Latitude = 10,
            Longitude = 10,
            Radius = 10
        };

        // Act
        var content = JsonContent.Create(request);
        var response = await _client.PostAsync("profiles/explore", content);

        // Assert
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseString);
            throw;
        }
    }

    [Theory]
    [InlineData(-100, 10, 10)]
    [InlineData(100, 10, 10)]
    [InlineData(10, 200, 10)]
    [InlineData(10, -200, 10)]
    [InlineData(10, 10, -1)]
    [InlineData(10, 10, 21000)]
    public async Task Explore_ReturnsValidationErrorWhenIncorrectRequest(double latitude, double longitude, double radius)
    {
        // Arrange
        var request = new ExploreRequest
        {
            Latitude = latitude,
            Longitude = longitude,
            Radius = radius
        };

        // Act
        var content = JsonContent.Create(request);
        var response = await _client.PostAsync("profiles/explore", content);

        // Assert
        var returnedJson = await response.Content.ReadAsStringAsync();
        try
        {
            var errorResponse = JsonConvert.DeserializeObject<ValidationErrorResponse>(returnedJson)!;
            Assert.False(errorResponse.Success);
            Assert.Single(errorResponse.Errors);
        }
        catch (Exception)
        {
            _output.WriteLine(returnedJson);
            throw;
        }
    }

    [Fact]
    public async Task Explore_ReturnsValidNubmerOfProfiles()
    {
        // Arrange
        var request = new ExploreRequest
        {
            Latitude = 10,
            Longitude = 10,
            Radius = 5000
        };

        // Act
        var content = JsonContent.Create(request);
        var response = await _client.PostAsync("profiles/explore", content);

        // Assert
        var returnedJson = await response.Content.ReadAsStringAsync();
        try
        {
            response.EnsureSuccessStatusCode();
            var profiles = JsonConvert.DeserializeObject<ExploreResponse>(returnedJson)!;
            Assert.True(profiles.Success);
            Assert.Equal(10, profiles.Profiles.Count);
        }
        catch (Exception)
        {
            _output.WriteLine(returnedJson);
            throw;
        }
    }
}