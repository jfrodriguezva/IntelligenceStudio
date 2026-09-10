namespace Mis.Domain.Acquisition;

public sealed class ProviderEntityMapping
{
    private ProviderEntityMapping() { }

    public ProviderEntityMapping(Guid id, string provider, string resourceType, string externalId, Guid canonicalId)
    {
        Id = id;
        Provider = provider;
        ResourceType = resourceType;
        ExternalId = externalId;
        CanonicalId = canonicalId;
    }

    public Guid Id { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string ResourceType { get; private set; } = string.Empty;
    public string ExternalId { get; private set; } = string.Empty;
    public Guid CanonicalId { get; private set; }
}
