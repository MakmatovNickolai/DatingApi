using IntegrationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;
namespace UnitTests.Auth.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri(TestConstants.ApiUrl)
        });
        _output = output;
    }

    //[Fact]
    public async Task RegisterEmail_ReturnsCorrectStatusCode()
    {
        // Arrange

        // todo move to unit tests

        //using var scope = _factory.Services.CreateScope();
        //var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        //var images = unitOfWork.ImageRepository.GetAll().Take(2).Select(x => x.Id).ToArray();
        //var request = FakeRequest.RegisterEmail(images);

        //// Act
        //var content = JsonContent.Create(request);
        //var response = await _client.PostAsync($"api/v{apiVersion}/auth/register/email", content);

        //// Assert
        //var returnedJson = await response.Content.ReadAsStringAsync();
        //try
        //{
        //    response.EnsureSuccessStatusCode();
        //    var returnedResponse = JsonConvert.DeserializeObject<RegisterResponse>(returnedJson);
        //    Assert.NotNull(returnedResponse);
        //    Assert.IsType<RegisterResponse>(returnedResponse);
        //    Assert.True(returnedResponse.Success);
        //}
        //catch (HttpRequestException ex)
        //{
        //    _output.WriteLine(returnedJson);
        //    throw;
        //}
    }
}