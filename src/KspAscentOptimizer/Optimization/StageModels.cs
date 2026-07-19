using System.Collections.Generic;

namespace KspAscentOptimizer.Optimization;

public enum GuidanceMode
{
    AscentToOrbit,
    SurfaceHop,
}

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

public sealed class SurfaceHopGuidanceTarget
{
    public string BodyName { get; set; } = string.Empty;
    public string DepartureName { get; set; } = string.Empty;
    public string DestinationName { get; set; } = string.Empty;
    public double DepartureLatitudeDegrees { get; set; }
    public double DepartureLongitudeDegrees { get; set; }
    public double DestinationLatitudeDegrees { get; set; }
    public double DestinationLongitudeDegrees { get; set; }
}

public sealed class GuidanceRequest
{
    public GuidanceMode Mode { get; set; }
    public double TargetOrbitAltitudeMeters { get; set; }
    public SurfaceHopGuidanceTarget? SurfaceHopTarget { get; set; }
}