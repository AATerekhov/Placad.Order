namespace OrderService.Infrastructure.ExternalServices;

/// <summary>
/// Fetches price snapshots from the Catalog microservice.
/// Currently returns stub data; replace with a real HttpClient when the Catalog service is available.
/// </summary>
public sealed class CatalogServiceClient
{
    public Task<decimal> GetPriceAsync(Guid planId, CancellationToken ct = default)
    {
        // TODO: call Catalog service REST API
        return Task.FromResult(9.99m);
    }
}
