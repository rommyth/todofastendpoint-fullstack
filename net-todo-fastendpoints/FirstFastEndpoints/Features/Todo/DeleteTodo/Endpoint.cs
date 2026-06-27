using FastEndpoints;
using FirstFastEndpoints.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FirstFastEndpoints.Features.Todo.DeleteTodo
{
    public class DeleteTodoEndpoint(AppDbContext db): EndpointWithoutRequest<DeleteTodoResponse>
    {
        public override void Configure()
        {
            Delete("/todos/{id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<Guid>("id");
            
            var result = await db.TodoItem.FirstOrDefaultAsync(x => x.Id == id, ct);

            if (result is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            db.Remove(result);
            await db.SaveChangesAsync(ct);

            await Send.OkAsync(ct);

        }
    }
}
