using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Services.Interfaces;

public interface IMerchantIdentificationService
{
    /// <summary>
    /// This method takes in all the merchant regex items and iterates through applying the regex to the merchant name.
    /// When a match is found, the merchant is identified, and the merchant domain object is retrieved from the repo.
    /// </summary>
    /// <param name="merchantName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Merchant?> IdentifyMerchantByRegexAsync(string merchantName, CancellationToken cancellationToken);
}