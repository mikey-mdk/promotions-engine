namespace PromotionsEngine.Infrastructure.ChangeFeed.Interfaces;

public interface ICosmosChangeFeedProcessor
{
    /// <summary>
    /// This method is used to setup the Cosmos Change Feed Processors.
    /// The change feed processor listens to changes on the specified cosmos containers and invalidates the cache accordingly.
    /// This method is only intended to be called during app start.
    /// </summary>
    /// <returns></returns>
    Task SetupCosmosChangeFeedProcessors();
}