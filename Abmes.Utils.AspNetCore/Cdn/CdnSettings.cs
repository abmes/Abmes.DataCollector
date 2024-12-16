namespace Abmes.Utils.AspNetCore.Cdn;

public record CdnSettings : ICdnSettings
{
    public string? CdnUrl { get; init; }
}
