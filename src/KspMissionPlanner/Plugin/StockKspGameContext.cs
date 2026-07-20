#if KSP_RUNTIME
using System;
using System.Collections.Generic;
using KspIntegration.Models;
using KspIntegration.Mission;
using ModelOrbitSnapshot = KspIntegration.Models.OrbitSnapshot;

namespace KspMissionPlanner.Plugin;

public sealed class StockKspGameContext : IKspGameContext
{
    public double UniversalTimeSeconds => Planetarium.GetUniversalTime();

    public string CurrentSaveName => HighLogic.CurrentGame?.Title ?? string.Empty;

    public GameStateSnapshot CaptureGameState()
    {
        var bodies = new List<CelestialBodySnapshot>();
        foreach (var body in FlightGlobals.Bodies)
        {
            bodies.Add(new CelestialBodySnapshot
            {
                Name = body.bodyName,
                ReferenceBodyName = body.referenceBody?.bodyName ?? string.Empty,
                GravitationalParameter = body.gravParameter,
                RadiusMeters = body.Radius,
                SphereOfInfluenceMeters = body.sphereOfInfluence,
                RotationPeriodSeconds = body.rotationPeriod,
                AtmosphereDepthMeters = body.atmosphereDepth,
                HasAtmosphere = body.atmosphere,
            });
        }

        VesselSnapshot? activeVessel = null;
        if (FlightGlobals.ActiveVessel is not null)
        {
            activeVessel = MapVessel(FlightGlobals.ActiveVessel);
        }

        return new GameStateSnapshot
        {
            SaveName = CurrentSaveName,
            UniversalTimeSeconds = UniversalTimeSeconds,
            CelestialBodies = bodies,
            ActiveVessel = activeVessel,
        };
    }

    private static VesselSnapshot MapVessel(Vessel vessel)
    {
        return new VesselSnapshot
        {
            Name = vessel.vesselName,
            BodyName = vessel.mainBody?.bodyName ?? string.Empty,
            TotalMassKg = vessel.GetTotalMass() * 1000.0,
            DryMassKg = ComputeDryMassKg(vessel),
            SurfaceSpeedMetersPerSecond = vessel.srfSpeed,
            OrbitalSpeedMetersPerSecond = vessel.obt_speed,
            AltitudeMeters = vessel.altitude,
            Orbit = vessel.orbit is null ? null : new ModelOrbitSnapshot
            {
                ReferenceBodyName = vessel.mainBody?.bodyName ?? string.Empty,
                InclinationRadians = vessel.orbit.inclination * Math.PI / 180.0,
                Eccentricity = vessel.orbit.eccentricity,
                SemiMajorAxisMeters = vessel.orbit.semiMajorAxis,
                PeriapsisAltitudeMeters = vessel.orbit.PeA,
                ApoapsisAltitudeMeters = vessel.orbit.ApA,
                LongitudeOfAscendingNodeRadians = vessel.orbit.LAN * Math.PI / 180.0,
                ArgumentOfPeriapsisRadians = vessel.orbit.argumentOfPeriapsis * Math.PI / 180.0,
                MeanAnomalyAtEpochRadians = vessel.orbit.meanAnomalyAtEpoch,
                EpochSeconds = vessel.orbit.epoch,
            },
            Stages = Array.Empty<StageSnapshot>(),
        };
    }

    private static double ComputeDryMassKg(Vessel vessel)
    {
        var dryMassTons = 0.0;
        foreach (var part in vessel.parts)
        {
            dryMassTons += part.mass;
        }

        return dryMassTons * 1000.0;
    }
}
#endif