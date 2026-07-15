using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using FirstFastEndpoints.Domain.Entities;
using FirstFastEndpoints.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace FirstFastEndpoints.Features.Auth.Register
{
    public class RegisterEndpoint(AppDbContext db, ILogger<RegisterEndpoint> logger) : Endpoint<RegisterRequest, RegisterResponse>
    {
        public override void Configure()
        {
            Post("register");
            AllowAnonymous();

            Throttle(hitLimit: 20, durationSeconds: 60);
        }

        public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
        {
            logger.LogInformation("Register attempt for {Email}", req.Email);

            var exist = await db.Users.AnyAsync(x => x.Email == req.Email);
            if (exist)
            {
                AddError(x => x.Email, "Email sudah digunakan");
                logger.LogWarning("Register attempt failed for {Email}", req.Email);
                await Send.ErrorsAsync();
                return;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = req.Name,
                Email = req.Email,
            };
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, req.Password);

            user.Password = hashedPassword;

            db.Users.Add(user);
            await db.SaveChangesAsync(ct);

            logger.LogInformation("User registered successfully: {Email}", req.Email);

            await Send.OkAsync(new RegisterResponse
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password
            }, ct);
        }
    }
}
