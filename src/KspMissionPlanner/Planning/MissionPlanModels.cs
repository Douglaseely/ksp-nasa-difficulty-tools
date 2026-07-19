using System;
using System.Collections.Generic;

namespace KspMissionPlanner.Planning;

public enum MissionObjectiveType
{
    Orbit,
    Flyby,
    Landing,
    SurfaceWaypoint,
    SurfaceHop,
}

public enum MissionLegType
{
    Launch,
    OrbitalTransfer,
    Flyby,
    Capture,
    Landing,
    SurfaceHop,
    ScienceWindow,
}

public sealed class SurfaceTarget
{
    public string BodyName { get; set; } = string.Empty;
    public string WaypointName { get; set; } = string.Empty;
    public double LatitudeDegrees { get; set; }
    public double LongitudeDegrees { get; set; }
    public double AltitudeMeters { get; set; }
}

public sealed class MissionObjective
{
    public string Name { get; set; } = string.Empty;
    public string TargetBody { get; set; } = string.Empty;
    public MissionObjectiveType ObjectiveType { get; set; }
    public SurfaceTarget? SurfaceTarget { get; set; }
}

public sealed class MissionLeg
{
    public MissionLegType LegType { get; set; }
    public string Description { get; set; } = string.Empty;
    public SurfaceTarget? DepartureSurfaceTarget { get; set; }
    public SurfaceTarget? ArrivalSurfaceTarget { get; set; }
    public double EstimatedDeltaVMetersPerSecond { get; set; }
    public double EstimatedCoastTimeSeconds { get; set; }
}

public sealed class PlannedManeuver
{
    public string Description { get; set; } = string.Empty;
    public double DeltaVMetersPerSecond { get; set; }
    public double BurnDurationSeconds { get; set; }
    public DateTimeOffset PlannedTimeUtc { get; set; }
}

public sealed class MissionPlan
{
    public MissionObjective Objective { get; set; } = new MissionObjective();
    public IReadOnlyList<MissionLeg> Legs { get; set; } = Array.Empty<MissionLeg>();
    public IReadOnlyList<PlannedManeuver> Maneuvers { get; set; } = Array.Empty<PlannedManeuver>();
}