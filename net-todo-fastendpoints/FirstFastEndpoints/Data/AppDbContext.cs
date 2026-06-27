using FirstFastEndpoints.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FirstFastEndpoints.Infrastructure
{
    public class AppDbContext(DbContextOptions options): DbContext(options) 
    {
        public DbSet<TodoItem> TodoItem { get; set; } 
    }
}
