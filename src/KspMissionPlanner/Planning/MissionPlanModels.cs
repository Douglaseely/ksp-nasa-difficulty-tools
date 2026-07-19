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

public enum MissionGoalType
{
    Orbit,
    Flyby,
    Landing,
    SurfaceWaypoint,
    SurfaceHop,
    ParkingOrbit,
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

public enum ParkingOrbitRole
{
    DescentStaging,
    AscentStaging,
}

public sealed class SurfaceTarget
{
    public string BodyName { get; set; } = string.Empty;
    public string WaypointName { get; set; } = string.Empty;
    public double LatitudeDegrees { get; set; }
    public double LongitudeDegrees { get; set; }
    public double AltitudeMeters { get; set; }
}

public sealed class OrbitTarget
{
    public string BodyName { get; set; } = string.Empty;
    public double PeriapsisAltitudeMeters { get; set; }
    public double ApoapsisAltitudeMeters { get; set; }
    public double InclinationRadians { get; set; }
}

public sealed class MissionStartPoint
{
    public string BodyName { get; set; } = string.Empty;
    public OrbitTarget? Orbit { get; set; }
    public SurfaceTarget? Surface { get; set; }
}

public sealed class MissionGoal
{
    public string GoalId { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public MissionGoalType GoalType { get; set; }
    public string TargetBody { get; set; } = string.Empty;
    public OrbitTarget? OrbitTarget { get; set; }
    public SurfaceTarget? SurfaceTarget { get; set; }
    public bool IsRequired { get; set; } = true;
}

public sealed class MissionStage
{
    public int StageNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<MissionGoal> Goals { get; set; } = Array.Empty<MissionGoal>();
}

public sealed class MissionProgram
{
    public string MissionId { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;
    public MissionStartPoint StartPoint { get; set; } = new MissionStartPoint();
    public IReadOnlyList<MissionStage> Stages { get; set; } = Array.Empty<MissionStage>();
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class GravityAssistOption
{
    public string AssistBodyName { get; set; } = string.Empty;
    public double EncounterWindowStartUtSeconds { get; set; }
    public double EncounterWindowEndUtSeconds { get; set; }
    public double EstimatedDeltaVSavingsMetersPerSecond { get; set; }
    public double EstimatedFlightTimeChangeSeconds { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public sealed class MissionTransitionAssessment
{
    public string FromGoalId { get; set; } = string.Empty;
    public string ToGoalId { get; set; } = string.Empty;
    public double DirectTransferDeltaVMetersPerSecond { get; set; }
    public double DirectTransferFlightTimeSeconds { get; set; }
    public IReadOnlyList<GravityAssistOption> GravityAssistCandidates { get; set; } = Array.Empty<GravityAssistOption>();
}

public sealed class ParkingOrbitTemplate
{
    public double DefaultPeriapsisAltitudeMeters { get; set; }
    public double DefaultApoapsisAltitudeMeters { get; set; }
    public double DefaultInclinationRadians { get; set; }
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
    public string FromGoalId { get; set; } = string.Empty;
    public string ToGoalId { get; set; } = string.Empty;
    public SurfaceTarget? DepartureSurfaceTarget { get; set; }
    public SurfaceTarget? ArrivalSurfaceTarget { get; set; }
    public double EstimatedDeltaVMetersPerSecond { get; set; }
    public double EstimatedCoastTimeSeconds { get; set; }
    public IReadOnlyList<GravityAssistOption> GravityAssistCandidates { get; set; } = Array.Empty<GravityAssistOption>();
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
    public string MissionId { get; set; } = string.Empty;
    public string MissionName { get; set; } = string.Empty;
    public MissionObjective Objective { get; set; } = new MissionObjective();
    public IReadOnlyList<MissionStage> Stages { get; set; } = Array.Empty<MissionStage>();
    public IReadOnlyList<MissionTransitionAssessment> TransitionAssessments { get; set; } = Array.Empty<MissionTransitionAssessment>();
    public IReadOnlyList<MissionLeg> Legs { get; set; } = Array.Empty<MissionLeg>();
    public IReadOnlyList<PlannedManeuver> Maneuvers { get; set; } = Array.Empty<PlannedManeuver>();
}