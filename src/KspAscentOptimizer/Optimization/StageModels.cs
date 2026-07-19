using System.Collections.Generic;

namespace KspAscentOptimizer.Optimization;

public sealed class StagePerformance
{
    public string StageName { get; init; } = string.Empty;
    public double MinThrottle01 { get; init; }
    public double MaxThrustNewton { get; init; }
    public double IspVacSeconds { get; init; }
    public double IspAtmSeconds { get; init; }
}

public sealed class VehicleProfile
{
    public string CraftName { get; init; } = string.Empty;
    public IReadOnlyList<StagePerformance> Stages { get; init; } = new List<StagePerformance>();
}