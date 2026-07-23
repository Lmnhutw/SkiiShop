using Microsoft.AspNetCore.Authentication;

namespace SShopAPI.Security;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string Scheme = "AdminApiKey";
    public const string HeaderName = "X-Admin-Key";

    public string? ApiKey { get; set; }
}
