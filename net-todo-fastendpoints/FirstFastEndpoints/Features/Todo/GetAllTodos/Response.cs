namespace FirstFastEndpoints.Features.Todo.GetAllTodos
{
    public class GetAllTodoResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }    
}
