using System;
using System.Collections.Generic;
using KspIntegration.Models;
using KspIntegration.Reflection;

namespace KspIntegration.Mission;

public sealed class ReflectionKspGameContext : IKspGameContext
{
    private readonly object _runtimeContext;

    public ReflectionKspGameContext(object runtimeContext)
    {
        _runtimeContext = runtimeContext ?? throw new ArgumentNullException(nameof(runtimeContext));
    }

    public double UniversalTimeSeconds => new ReflectionObjectReader(_runtimeContext)
        .GetOptionalDouble(0.0, "UniversalTimeSeconds", "universalTimeSeconds", "UT");

    public string CurrentSaveName => new ReflectionObjectReader(_runtimeContext)
        .GetOptionalString(string.Empty, "CurrentSaveName", "currentSaveName", "SaveName", "saveName");

    public GameStateSnapshot CaptureGameState()
    {
        var root = new ReflectionObjectReader(_runtimeContext);
        var bodies = new List<CelestialBodySnapshot>();

        foreach (var body in root.GetOptionalObjectList("CelestialBodies", "celestialBodies", "Bodies", "bodies"))
        {
            bodies.Add(MapBody(body));
        }

        VesselSnapshot? activeVessel = null;
        var activeVesselObject = root.GetOptionalObject("ActiveVessel", "activeVessel", "Vessel", "vessel");
        if (activeVesselObject is not null)
        {
            activeVessel = MapVessel(activeVesselObject);
        }

        return new GameStateSnapshot
        {
            SaveName = CurrentSaveName,
            UniversalTimeSeconds = UniversalTimeSeconds,
            CelestialBodies = bodies,
            ActiveVessel = activeVessel,
        };
    }

    private static CelestialBodySnapshot MapBody(object runtimeBody)
    {
        var reader = new ReflectionObjectReader(runtimeBody);
        return new CelestialBodySnapshot
        {
            Name = reader.GetOptionalString(string.Empty, "Name", "name", "bodyName"),
            ReferenceBodyName = reader.GetOptionalString(string.Empty, "ReferenceBodyName", "referenceBodyName", "ParentBodyName", "parentBodyName"),
            GravitationalParameter = reader.GetOptionalDouble(0.0, "GravitationalParameter", "gravParameter", "Mu", "mu"),
            RadiusMeters = reader.GetOptionalDouble(0.0, "RadiusMeters", "Radius", "radius"),
            SphereOfInfluenceMeters = reader.GetOptionalDouble(0.0, "SphereOfInfluenceMeters", "sphereOfInfluence", "SphereOfInfluence"),
            RotationPeriodSeconds = reader.GetOptionalDouble(0.0, "RotationPeriodSeconds", "rotationPeriod", "RotationPeriod"),
            AtmosphereDepthMeters = reader.GetOptionalDouble(0.0, "AtmosphereDepthMeters", "atmosphereDepth", "AtmosphereDepth"),
            HasAtmosphere = reader.GetOptionalBoolean(false, "HasAtmosphere", "atmosphere", "hasAtmosphere"),
        };
    }

    private static VesselSnapshot MapVessel(object runtimeVessel)
    {
        var reader = new ReflectionObjectReader(runtimeVessel);
        var stages = new List<StageSnapshot>();
        foreach (var stage in reader.GetOptionalObjectList("Stages", "stages", "StageData", "stageData"))
        {
            stages.Add(MapStage(stage));
        }

        OrbitSnapshot? orbit = null;
        var orbitObject = reader.GetOptionalObject("Orbit", "orbit");
        if (orbitObject is not null)
        {
            orbit = MapOrbit(orbitObject);
        }

        return new VesselSnapshot
        {
            Name = reader.GetOptionalString(string.Empty, "Name", "name", "VesselName", "vesselName"),
            BodyName = reader.GetOptionalString(string.Empty, "BodyName", "bodyName", "MainBodyName", "mainBodyName"),
            TotalMassKg = reader.GetOptionalDouble(0.0, "TotalMassKg", "totalMassKg", "Mass", "mass"),
            DryMassKg = reader.GetOptionalDouble(0.0, "DryMassKg", "dryMassKg", "DryMass", "dryMass"),
            SurfaceSpeedMetersPerSecond = reader.GetOptionalDouble(0.0, "SurfaceSpeedMetersPerSecond", "surfaceSpeedMetersPerSecond", "srfSpeed"),
            OrbitalSpeedMetersPerSecond = reader.GetOptionalDouble(0.0, "OrbitalSpeedMetersPerSecond", "orbitalSpeedMetersPerSecond", "obt_speed"),
            AltitudeMeters = reader.GetOptionalDouble(0.0, "AltitudeMeters", "altitudeMeters", "altitude"),
            Orbit = orbit,
            Stages = stages,
        };
    }

    private static OrbitSnapshot MapOrbit(object runtimeOrbit)
    {
        var reader = new ReflectionObjectReader(runtimeOrbit);
        return new OrbitSnapshot
        {
            ReferenceBodyName = reader.GetOptionalString(string.Empty, "ReferenceBodyName", "referenceBodyName", "BodyName", "bodyName"),
            InclinationRadians = reader.GetOptionalDouble(0.0, "InclinationRadians", "inclination", "Inclination"),
            Eccentricity = reader.GetOptionalDouble(0.0, "Eccentricity", "eccentricity", "ecc"),
            SemiMajorAxisMeters = reader.GetOptionalDouble(0.0, "SemiMajorAxisMeters", "semiMajorAxis", "semiMajorAxisMeters"),
            PeriapsisAltitudeMeters = reader.GetOptionalDouble(0.0, "PeriapsisAltitudeMeters", "PeA", "periapsisAltitudeMeters"),
            ApoapsisAltitudeMeters = reader.GetOptionalDouble(0.0, "ApoapsisAltitudeMeters", "ApA", "apoapsisAltitudeMeters"),
            LongitudeOfAscendingNodeRadians = reader.GetOptionalDouble(0.0, "LongitudeOfAscendingNodeRadians", "LAN", "longitudeOfAscendingNodeRadians"),
            ArgumentOfPeriapsisRadians = reader.GetOptionalDouble(0.0, "ArgumentOfPeriapsisRadians", "argumentOfPeriapsis", "ArgumentOfPeriapsis"),
            MeanAnomalyAtEpochRadians = reader.GetOptionalDouble(0.0, "MeanAnomalyAtEpochRadians", "meanAnomalyAtEpoch", "MeanAnomalyAtEpoch"),
            EpochSeconds = reader.GetOptionalDouble(0.0, "EpochSeconds", "epoch", "Epoch"),
        };
    }

    private static StageSnapshot MapStage(object runtimeStage)
    {
        var reader = new ReflectionObjectReader(runtimeStage);
        var engines = new List<EngineSnapshot>();
        foreach (var engine in reader.GetOptionalObjectList("Engines", "engines"))
        {
            engines.Add(MapEngine(engine));
        }

        return new StageSnapshot
        {
            StageIndex = reader.GetOptionalInt32(0, "StageIndex", "stageIndex", "Index", "index"),
            StageName = reader.GetOptionalString(string.Empty, "StageName", "stageName", "Name", "name"),
            StartMassKg = reader.GetOptionalDouble(0.0, "StartMassKg", "startMassKg", "StartMass", "startMass"),
            EndMassKg = reader.GetOptionalDouble(0.0, "EndMassKg", "endMassKg", "EndMass", "endMass"),
            MaxThrustNewton = reader.GetOptionalDouble(0.0, "MaxThrustNewton", "maxThrustNewton", "MaxThrust", "maxThrust"),
            MinThrottle01 = reader.GetOptionalDouble(0.0, "MinThrottle01", "minThrottle", "MinThrottle"),
            IspVacSeconds = reader.GetOptionalDouble(0.0, "IspVacSeconds", "ispVacSeconds", "VacIsp", "vacIsp"),
            IspAtmSeconds = reader.GetOptionalDouble(0.0, "IspAtmSeconds", "ispAtmSeconds", "AtmIsp", "atmIsp"),
            RemainingIgnitions = reader.GetOptionalInt32(0, "RemainingIgnitions", "remainingIgnitions", "IgnitionsRemaining", "ignitionsRemaining"),
            Engines = engines,
        };
    }

    private static EngineSnapshot MapEngine(object runtimeEngine)
    {
        var reader = new ReflectionObjectReader(runtimeEngine);
        return new EngineSnapshot
        {
            EngineName = reader.GetOptionalString(string.Empty, "EngineName", "engineName", "Name", "name"),
            MaxThrustNewton = reader.GetOptionalDouble(0.0, "MaxThrustNewton", "maxThrustNewton", "MaxThrust", "maxThrust"),
            MinThrottle01 = reader.GetOptionalDouble(0.0, "MinThrottle01", "minThrottle", "MinThrottle"),
            IspVacSeconds = reader.GetOptionalDouble(0.0, "IspVacSeconds", "ispVacSeconds", "VacIsp", "vacIsp"),
            IspAtmSeconds = reader.GetOptionalDouble(0.0, "IspAtmSeconds", "ispAtmSeconds", "AtmIsp", "atmIsp"),
            RemainingIgnitions = reader.GetOptionalInt32(0, "RemainingIgnitions", "remainingIgnitions", "IgnitionsRemaining", "ignitionsRemaining"),
            RequiresUllage = reader.GetOptionalBoolean(false, "RequiresUllage", "requiresUllage"),
            PressureFed = reader.GetOptionalBoolean(false, "PressureFed", "pressureFed"),
        };
    }
}