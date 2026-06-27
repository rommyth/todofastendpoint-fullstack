using FastEndpoints;
using FluentValidation;

namespace FirstFastEndpoints.Features.Todo.CreateTodo
{
    public class CreateTodoValidation : Validator<CreateTodoRequest>
    {
        public CreateTodoValidation()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(1).WithMessage("Title must be at least 1 character long.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)               
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters.");
        }
    }
}
