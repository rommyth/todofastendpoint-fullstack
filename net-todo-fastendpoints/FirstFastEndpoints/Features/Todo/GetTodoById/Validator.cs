using FastEndpoints;
using FluentValidation;

namespace FirstFastEndpoints.Features.Todo.GetTodoById
{
    public class GetTodoByIdValidator : Validator<GetTodoByIdRequest>
    {
        public GetTodoByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .Must(id => Guid.TryParse(id.ToString(), out _))
                .WithMessage("Invalid id format");
        }
    }
}
