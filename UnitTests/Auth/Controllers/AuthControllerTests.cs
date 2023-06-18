using Auth.Dto;
using Auth.Services;
using Common.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace UnitTests.Auth.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IRegisterService> _registerServiceMock = new();

    private const string apiVersion = "1";

    private readonly TestServer _server;
    private readonly HttpClient _client;

    public AuthControllerTests()
    {
        var app = WebApplication.Create();
        // Configure your API's endpoints and middleware directly within the test class
        app.MapPost("api/{apiVersion}/auth/register/email", async context =>
        {
            // Process the request and return a response
            var requestBody = await context.Request.ReadFromJsonAsync<RegisterEmailRequest>();
            var response = new SuccessfulResponse("Registration successful");
            await context.Response.WriteAsJsonAsync(response);
        });


        _server = app.GetTestServer();
        _client = _server.CreateClient();
    }

    //[Fact]
    public async Task RegisterEmail_HappyPath()
    {
        // todo https://medium.com/technology-earnin/thorough-testing-of-asp-net-core-or-any-web-api-a87bd0585f9b
        // https://timdeschryver.dev/blog/how-to-test-your-csharp-web-api#a-simple-test

        var registerEmailDto = new RegisterEmailRequest
        {

        };

        var result = new SuccessfulResponse("Registration successful");

        _registerServiceMock
            .Setup(service => service.RegisterEmailAsync(registerEmailDto))
            .Verifiable();

        var content = JsonContent.Create(registerEmailDto);
        var response = await _client.PostAsync($"api/{apiVersion}/auth/register/email", content);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var returnedJson = await response.Content.ReadAsStringAsync();
        var returnedResponse = JsonConvert.DeserializeObject<SuccessfulResponse>(returnedJson);
        Assert.Equal(result, returnedResponse);
    }
}
