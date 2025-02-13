using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace PromotionsEngine.Tests.Integration.TestBase;

public class IntegrationTestBase : IDisposable
{
    public IntegrationTestBase()
    {
        var factory = new CustomWebApplicationFactory<Program>();

        HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    public HttpClient HttpClient { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        HttpClient.Dispose();
    }
}