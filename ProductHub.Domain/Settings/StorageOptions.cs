namespace ProductHub.Domain.Settings;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public required string ConnectionString { get; init; }

    public required string ContainerName { get; init; }
}
