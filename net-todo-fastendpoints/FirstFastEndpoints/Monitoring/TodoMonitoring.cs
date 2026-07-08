using System.Diagnostics.Metrics;

namespace FirstFastEndpoints.Monitoring;

public static class TodoMonitoring
{
    public static readonly Meter meter = new Meter("Todo.Api", "1.0.0");

    public static readonly Counter<int> TodoCreated = meter.CreateCounter<int>("todo_created_total");

    public static readonly Counter<int> TodoDeleted = meter.CreateCounter<int>("todo_deleted_total");

    public static readonly Counter<int> TodoUpdated = meter.CreateCounter<int>("todo_updated_total");
}
