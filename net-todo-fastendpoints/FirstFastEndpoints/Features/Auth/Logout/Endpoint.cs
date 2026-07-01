using FastEndpoints;
using FirstFastEndpoints.Infrastructure;
using FirstFastEndpoints.Shared.Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FirstFastEndpoints.Features.Auth.Logout;

public class LogoutEnpoint(AppDbContext db, ICacheService cache) : EndpointWithoutRequest<LogoutResponse>
{
    public override void Configure()
    {
        Post("/logout");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var jti = User.FindFirst("jti")?.Value;
        if (string.IsNullOrWhiteSpace(jti))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var exp = User.FindFirst("exp")?.Value;
        if (string.IsNullOrWhiteSpace(exp))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var expUnix = long.Parse(exp);
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix);

        var ttl = expiresAt - DateTimeOffset.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            await Send.OkAsync(ct);
            return;
        }

        await cache.SetAsync(CacheKeys.TokenBlockId(Guid.Parse(jti)), "1", ttl);

        await Send.OkAsync(ct);
    }
}
