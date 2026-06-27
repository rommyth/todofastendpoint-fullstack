using FastEndpoints;

namespace FirstFastEndpoints.Features.Todo.GetTodoById
{
    public class GetTodoByIdRequest
    {
        [BindFrom("id")]
        public Guid Id { get; set; }
    }
}
