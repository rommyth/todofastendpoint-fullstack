using FastEndpoints;
using FirstFastEndpoints.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FirstFastEndpoints.Features.Todo.GetTodoById
{
    public class GetTodoByIdEndpoint(AppDbContext db) : Endpoint<GetTodoByIdRequest, GetTodoByIdResponse>
    {
        public override void Configure()
        {
            Get("/todos/{id}");
            AllowAnonymous();

            Description(o => o
                 .Produces<GetTodoByIdResponse>(200)
                 .ProducesProblem(404)
            );
        }

        public override async Task HandleAsync(GetTodoByIdRequest req, CancellationToken ct)
        {
            var result = await db.TodoItem
                .Where(x => x.Id == req.Id)
                .Select(q => new GetTodoByIdResponse
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    IsCompleted = q.IsCompleted,
                    UserId = q.UserId,
                    UserName = q.User.Name,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt
                }).FirstOrDefaultAsync(ct);

            if (result is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            await Send.OkAsync(result, cancellation: ct);
        }
    }
}
