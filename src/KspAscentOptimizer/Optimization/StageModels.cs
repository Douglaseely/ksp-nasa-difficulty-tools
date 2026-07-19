using System.Collections.Generic;

namespace KspAscentOptimizer.Optimization;

public sealed class StagePerformance
{
    public string StageName { get; set; } = string.Empty;
    public double MinThrottle01 { get; set; }
    public double MaxThrustNewton { get; set; }
    public double IspVacSeconds { get; set; }
    public double IspAtmSeconds { get; set; }
}

public sealed class VehicleProfile
{
    public string CraftName { get; set; } = string.Empty;
    public IReadOnlyList<StagePerformance> Stages { get; set; } = new List<StagePerformance>();
}