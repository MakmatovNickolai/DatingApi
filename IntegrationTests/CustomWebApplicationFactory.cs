using DataAccess.Repositories;
using FakeData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(defaultScheme: "TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme, options => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(
                        TestAuthHandler.AuthenticationScheme)
                    .Build();
            });

            EnsureTestUserCreated(services);
        });

        builder.UseEnvironment("Development");
    }

    private void EnsureTestUserCreated(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var unitOfWork = scopedServices.GetRequiredService<IUnitOfWork>();
            var testUser = unitOfWork.UserRepository.GetAll().FirstOrDefault(p => p.IsBlocked && p.Email == "testEmail");
            if (testUser is null)
            {
                testUser = FakeUser.GetNext();
                testUser.Email = "testEmail";
                testUser.IsBlocked = true;
                unitOfWork.UserRepository.InsertAsync(testUser).Wait();
                var gender = unitOfWork.GenderRepository.GetAll().First();
                var testProfile = FakeProfile.GetNext(testUser.Id, gender.Id);
                unitOfWork.ProfileRepository.InsertAsync(testProfile).Wait();
            }
        }
    }
}