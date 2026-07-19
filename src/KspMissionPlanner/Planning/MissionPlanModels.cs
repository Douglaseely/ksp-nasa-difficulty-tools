using System;
using System.Collections.Generic;

namespace KspMissionPlanner.Planning;

public sealed class MissionObjective
{
    public string Name { get; set; } = string.Empty;
    public string TargetBody { get; set; } = string.Empty;
    public string ObjectiveType { get; set; } = string.Empty;
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
    public IReadOnlyList<PlannedManeuver> Maneuvers { get; set; } = Array.Empty<PlannedManeuver>();
}