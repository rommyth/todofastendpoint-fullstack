using FastEndpoints;
using FirstFastEndpoints.Infrastructure;
using FirstFastEndpoints.Shared.Caching;
using Microsoft.EntityFrameworkCore;

namespace FirstFastEndpoints.Features.Todo.GetAllTodos
{
    public class GetAllTodoEndpoint(AppDbContext db, ICacheService cache) : EndpointWithoutRequest<List<GetAllTodoResponse>>
    {
        public override void Configure()
        {
            Get("/todos");
            AllowAnonymous();


            Summary(s =>
            {
                s.Description = "Somehow there any description here to get All Todos";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var cacheKey = CacheKeys.TodoList;
            var cachedTodos = await cache.GetAsync<List<GetAllTodoResponse>>(cacheKey);
            if (cachedTodos is not null)
            {
                await Send.OkAsync(cachedTodos, ct);
                return;
            }

            var todos = await db.TodoItem
                .AsNoTracking()
                .Select(q => new GetAllTodoResponse
                {
                    Id = q.Id,
                    Title = q.Title,
                    IsCompleted = q.IsCompleted,
                    UserId = q.UserId,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt
                }).ToListAsync(ct);

            await cache.SetAsync(CacheKeys.TodoList, todos, TimeSpan.FromSeconds(30));

            await Send.OkAsync(todos, ct);
        }
    }
}
