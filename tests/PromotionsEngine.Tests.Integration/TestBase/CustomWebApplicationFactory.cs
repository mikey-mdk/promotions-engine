using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace PromotionsEngine.Tests.Integration.TestBase;

public class CustomWebApplicationFactory<TProgram> : 
    WebApplicationFactory<TProgram> where TProgram: class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
        });

        builder.ConfigureTestServices(_ => {});

        builder.UseEnvironment("Development");
    }
}