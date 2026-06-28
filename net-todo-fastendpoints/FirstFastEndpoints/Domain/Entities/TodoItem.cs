namespace FirstFastEndpoints.Domain.Entities
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public bool IsCompleted { get; set; } = false;

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
