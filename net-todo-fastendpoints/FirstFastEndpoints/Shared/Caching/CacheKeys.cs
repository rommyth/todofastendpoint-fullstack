namespace FirstFastEndpoints.Shared.Caching;

public static class CacheKeys
{
    public const string TodoList = "todos:all";
    public static string TodoById(Guid id) => $"todos:{id}";
    public static string TokenBlockId(Guid id) => $"blacklist:{id}";
}
