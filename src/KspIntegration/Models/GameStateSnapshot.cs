using System;
using System.Collections.Generic;

namespace KspIntegration.Models;

public sealed class GameStateSnapshot
{
    public string SaveName { get; set; } = string.Empty;
    public double UniversalTimeSeconds { get; set; }
    public IReadOnlyList<CelestialBodySnapshot> CelestialBodies { get; set; } = Array.Empty<CelestialBodySnapshot>();
    public VesselSnapshot? ActiveVessel { get; set; }
}

public sealed class CelestialBodySnapshot
{
    public string Name { get; set; } = string.Empty;
    public string ReferenceBodyName { get; set; } = string.Empty;
    public double GravitationalParameter { get; set; }
    public double RadiusMeters { get; set; }
    public double SphereOfInfluenceMeters { get; set; }
    public double RotationPeriodSeconds { get; set; }
    public double AtmosphereDepthMeters { get; set; }
    public bool HasAtmosphere { get; set; }
}

public sealed class VesselSnapshot
{
    public string Name { get; set; } = string.Empty;
    public string BodyName { get; set; } = string.Empty;
    public double TotalMassKg { get; set; }
    public double DryMassKg { get; set; }
    public double SurfaceSpeedMetersPerSecond { get; set; }
    public double OrbitalSpeedMetersPerSecond { get; set; }
    public double AltitudeMeters { get; set; }
    public OrbitSnapshot? Orbit { get; set; }
    public IReadOnlyList<StageSnapshot> Stages { get; set; } = Array.Empty<StageSnapshot>();
}

public sealed class OrbitSnapshot
{
    public string ReferenceBodyName { get; set; } = string.Empty;
    public double InclinationRadians { get; set; }
    public double Eccentricity { get; set; }
    public double SemiMajorAxisMeters { get; set; }
    public double PeriapsisAltitudeMeters { get; set; }
    public double ApoapsisAltitudeMeters { get; set; }
    public double LongitudeOfAscendingNodeRadians { get; set; }
    public double ArgumentOfPeriapsisRadians { get; set; }
    public double MeanAnomalyAtEpochRadians { get; set; }
    public double EpochSeconds { get; set; }
}

public sealed class StageSnapshot
{
    public int StageIndex { get; set; }
    public string StageName { get; set; } = string.Empty;
    public double StartMassKg { get; set; }
    public double EndMassKg { get; set; }
    public double MaxThrustNewton { get; set; }
    public double MinThrottle01 { get; set; }
    public double IspVacSeconds { get; set; }
    public double IspAtmSeconds { get; set; }
    public int RemainingIgnitions { get; set; }
    public IReadOnlyList<EngineSnapshot> Engines { get; set; } = Array.Empty<EngineSnapshot>();
}

public sealed class EngineSnapshot
{
    public string EngineName { get; set; } = string.Empty;
    public double MaxThrustNewton { get; set; }
    public double MinThrottle01 { get; set; }
    public double IspVacSeconds { get; set; }
    public double IspAtmSeconds { get; set; }
    public int RemainingIgnitions { get; set; }
    public bool RequiresUllage { get; set; }
    public bool PressureFed { get; set; }
}

public sealed class WaypointSnapshot
{
    public string BodyName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BiomeName { get; set; } = string.Empty;
    public double LatitudeDegrees { get; set; }
    public double LongitudeDegrees { get; set; }
    public double AltitudeMeters { get; set; }
}

public sealed class ManeuverNodeRequest
{
    public double UniversalTimeSeconds { get; set; }
    public double ProgradeMetersPerSecond { get; set; }
    public double NormalMetersPerSecond { get; set; }
    public double RadialMetersPerSecond { get; set; }
}

public sealed class AerodynamicSnapshot
{
    public double DynamicPressurePa { get; set; }
    public double AtmosphericDensityKgPerCubicMeter { get; set; }
    public double MachNumber { get; set; }
    public double EstimatedDragLossesMetersPerSecond { get; set; }
}

public sealed class MechJebAscentSettingsSnapshot
{
    public double LimitQPa { get; set; }
    public double MaxAccelerationMetersPerSecondSquared { get; set; }
    public bool AscentAutopilotEnabled { get; set; }
    public bool SupportsSurfaceHopGuidance { get; set; }
}