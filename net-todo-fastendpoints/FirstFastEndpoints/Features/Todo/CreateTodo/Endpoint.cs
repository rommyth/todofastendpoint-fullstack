using FastEndpoints;
using FirstFastEndpoints.Domain.Entities;
using FirstFastEndpoints.Infrastructure;
using FirstFastEndpoints.Shared.PreProcessors;

namespace FirstFastEndpoints.Features.Todo.CreateTodo
{
    public class CreateTodoEndpoint(AppDbContext db) : EndpointWithMapping<CreateTodoRequest, CreateTodoResponse, TodoItem>
    {

        public override void Configure()
        {          
            Post("/todos");
            AllowAnonymous();
            PreProcessor<LoggiingPreProcessor<CreateTodoRequest>>();


            Throttle(hitLimit: 20, durationSeconds: 60);

            Description(o => o
                .Produces<CreateTodoResponse>(201)
                .ProducesProblem(400)
            );
        }

        public override async Task HandleAsync(CreateTodoRequest req, CancellationToken ct)
        {
            var todoItem = MapToEntity(req);
            db.TodoItem.Add(todoItem);
            await db.SaveChangesAsync(ct);
            var response = MapFromEntity(todoItem);

            await Send.CreatedAtAsync("/todos/{id}", new { Id = response.Id }, response, cancellation: ct);
        }

        public override TodoItem MapToEntity(CreateTodoRequest r)
        {
            return new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = r.Title,
                Description = r.Description,
            };
        }

        public override CreateTodoResponse MapFromEntity(TodoItem e)
        {
            return new CreateTodoResponse { Id = e.Id };
        }
    }
}
