using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using FirstFastEndpoints.Domain.Entities;
using FirstFastEndpoints.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FirstFastEndpoints.Features.Auth.Login
{
    public class LoginEndpoint(AppDbContext db, IConfiguration configuration) : Endpoint<LoginRequest, LoginResponse>
    {
        public override void Configure()
        {
            Post("/login");
            AllowAnonymous();

            Throttle(hitLimit: 20, durationSeconds: 60);
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == req.Email);
            if (user is null)
            {
                await Send.UnauthorizedAsync(ct);
                return;
            }

            var hasherPassword = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, req.Password);
            if (hasherPassword == PasswordVerificationResult.Failed)
            {
                await Send.UnauthorizedAsync(ct);
                return;
            }

            var jwtToken = JwtBearer.CreateToken(
                o =>
                {
                    o.SigningKey = configuration.GetValue<string>("Jwt:Key")!;
                    o.ExpireAt = DateTime.UtcNow.AddHours(1);
                    o.User.Claims.Add((ClaimTypes.NameIdentifier, user.Id.ToString()));
                    o.User.Claims.Add(("jti", Guid.NewGuid().ToString()));
                }
            );

            await Send.OkAsync(
                new LoginResponse
                {
                    AccessToken = jwtToken
                },
                cancellation: ct
            );
        }
    }
}
