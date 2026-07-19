using System;
using System.Collections.Generic;

namespace KspMissionPlanner.Planning;

public sealed class MissionObjective
{
    public string Name { get; init; } = string.Empty;
    public string TargetBody { get; init; } = string.Empty;
    public string ObjectiveType { get; init; } = string.Empty;
}

public sealed class PlannedManeuver
{
    public string Description { get; init; } = string.Empty;
    public double DeltaVMetersPerSecond { get; init; }
    public double BurnDurationSeconds { get; init; }
    public DateTimeOffset PlannedTimeUtc { get; init; }
}

public sealed class MissionPlan
{
    public MissionObjective Objective { get; init; } = new MissionObjective();
    public IReadOnlyList<PlannedManeuver> Maneuvers { get; init; } = Array.Empty<PlannedManeuver>();
}