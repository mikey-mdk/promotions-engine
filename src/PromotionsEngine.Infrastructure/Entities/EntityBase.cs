using System.Text.Json.Serialization;

namespace PromotionsEngine.Infrastructure.Entities;

/// <summary>
/// Base class for all entities. Its current purpose is to provide an Id property for all entities.
/// </summary>
public abstract class EntityBase
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}