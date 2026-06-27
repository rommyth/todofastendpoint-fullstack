using FastEndpoints;
using FirstFastEndpoints.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FirstFastEndpoints.Features.Todo.GetAllTodos
{
    public class GetAllTodoEndpoint(AppDbContext db) : EndpointWithoutRequest<List<GetAllTodoResponse>>
    {        
        public override void Configure()
        {
            Get("/todos");
            AllowAnonymous();


            Summary(s => {
                s.Description = "Somehow there any description here to get All Todos";                     
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = await db.TodoItem
                .AsNoTracking()
                .Select(q => new GetAllTodoResponse
            {
                Id = q.Id,
                Title = q.Title,
                IsCompleted = q.IsCompleted,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt
            }).ToListAsync(ct);

            await Send.OkAsync(result, ct);
        }

    }
}
