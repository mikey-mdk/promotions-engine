using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Repositories.Interfaces;

public interface IMerchantRegexRepository
{
    /// <summary>
    /// This method will query all the MerchantRegex entities from the database.
    /// </summary>
    /// <returns></returns>
    Task<List<MerchantRegex>> GetAllMerchantRegexItemsAsync();

    /// <summary>
    /// Create the MerchantRegex entity.
    /// </summary>
    /// <param name="merchantRegex"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MerchantRegex?> CreateMerchantRegexAsync(MerchantRegex merchantRegex, CancellationToken cancellationToken);

    /// <summary>
    /// Replaces the regex patterns list in the MerchantRegexEntity.
    /// </summary>
    /// <param name="merchantRegex"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MerchantRegex?> ReplaceRegexPatternsAsync(MerchantRegex merchantRegex, CancellationToken cancellationToken);
}