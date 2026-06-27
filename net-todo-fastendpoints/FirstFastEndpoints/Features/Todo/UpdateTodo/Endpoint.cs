using FastEndpoints;
using FirstFastEndpoints.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FirstFastEndpoints.Features.Todo.UpdateTodo
{
    public class UpdateTodoEndpoint(AppDbContext db): Endpoint<UpdateTodoRequest, UpdateTodoResponse>
    {
        public override void Configure()
        {
            Put("/todos/{id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(UpdateTodoRequest req, CancellationToken ct)
        {
            var id = Route<Guid>("id");

            var todo = await db.TodoItem.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (todo is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            todo.Title = req.Title ?? todo.Title;
            todo.Description = req.Description ?? todo.Description;
            todo.IsCompleted = req.IsCompleted ?? todo.IsCompleted;

            await db.SaveChangesAsync(ct);

            await Send.OkAsync(ct);
        }
    }
}
