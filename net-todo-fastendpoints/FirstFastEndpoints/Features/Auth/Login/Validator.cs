using FastEndpoints;
using FluentValidation;

namespace FirstFastEndpoints.Features.Auth.Login
{
    public class LoginValidator : Validator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
