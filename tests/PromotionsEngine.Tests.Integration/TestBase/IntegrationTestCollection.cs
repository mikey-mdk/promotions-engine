using Xunit;

namespace PromotionsEngine.Tests.Integration.TestBase;

[CollectionDefinition("Integration Test Collection")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestBase>
{
    //This class contains no code. It is a dummy placeholder class to facilitate providing a collection fixture.
}