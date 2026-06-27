namespace FirstFastEndpoints.Features.Todo.CreateTodo
{
    public class CreateTodoRequest
    {
        public string Title { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        public override string ToString()
        {
            return $"Title: {Title}, Description: {Description}";
        }
    }; 
}
