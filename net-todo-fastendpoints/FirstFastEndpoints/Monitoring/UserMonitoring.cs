using System.Diagnostics.Metrics;

namespace FirstFastEndpoints.Monitoring;

public static class UserMonitoring
{
    public static readonly Meter meter = new Meter("User.Api");

    public static readonly Counter<int> UserLogin = meter.CreateCounter<int>("user_login_total");
}
