using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using FirstFastEndpoints.Domain.Entities;
using FirstFastEndpoints.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace FirstFastEndpoints.Features.Auth.Register
{
    public class RegisterEndpoint(AppDbContext db) : Endpoint<RegisterRequest, RegisterResponse>
    {
        public override void Configure()
        {
            Post("register");
            AllowAnonymous();

            Throttle(hitLimit: 20, durationSeconds: 60);
        }

        public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
        {
            var exist = await db.Users.AnyAsync(x => x.Email == req.Email);
            if (exist)
            {
                AddError(x => x.Email, "Email sudah digunakan");
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
            // var verifyPassword = new PasswordHasher<User>().VerifyHashedPassword(user, hashedPassword, req.Password);

            user.Password = hashedPassword;

            db.Users.Add(user);
            await db.SaveChangesAsync(ct);

            await Send.OkAsync(new RegisterResponse
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password
            }, ct);
        }
    }
}
