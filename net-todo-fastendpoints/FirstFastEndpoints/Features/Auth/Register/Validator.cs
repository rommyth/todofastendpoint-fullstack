using FastEndpoints;
using FluentValidation;

namespace FirstFastEndpoints.Features.Auth.Register
{
    public class RegisterValidator : Validator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(4).WithMessage("Minimum 4 char");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MinimumLength(3).WithMessage("Minimum 3 char");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6).WithMessage("Minimum 6 char");
        }
    }
}
