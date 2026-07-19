using System.Collections.Generic;
using KspAscentOptimizer.Optimization;
using KspAscentOptimizer.Plugin;
using KspIntegration.Flight;
using KspIntegration.Mission;
using KspIntegration.Models;
using KspMissionPlanner.Planning;
using KspMissionPlanner.Plugin;
using Xunit;

namespace KspIntegrationTests;

public sealed class ReflectionAdapterTests
{
    [Fact]
    public void ReflectionKspGameContext_CapturesBodiesVesselOrbitAndStages()
    {
        var runtimeContext = new FakeGameContext
        {
            UniversalTimeSeconds = 123456.0,
            CurrentSaveName = "Career-NASA",
            CelestialBodies = new List<FakeBody>
            {
                new FakeBody { Name = "Kerbin", gravParameter = 3.5316e12, Radius = 600000, atmosphereDepth = 70000, atmosphere = true },
                new FakeBody { Name = "Mun", gravParameter = 6.5138398e10, Radius = 200000, atmosphereDepth = 0, atmosphere = false },
            },
            ActiveVessel = new FakeVessel
            {
                VesselName = "Mun Relay 1",
                MainBodyName = "Kerbin",
                Mass = 12_500,
                DryMass = 4_100,
                altitude = 82_000,
                srfSpeed = 2250,
                obt_speed = 2279,
                Orbit = new FakeOrbit
                {
                    BodyName = "Kerbin",
                    inclination = 0.02,
                    eccentricity = 0.001,
                    semiMajorAxis = 682000,
                    PeA = 80000,
                    ApA = 84000,
                    LAN = 0.3,
                    argumentOfPeriapsis = 1.1,
                    meanAnomalyAtEpoch = 0.4,
                    epoch = 123000,
                },
                Stages = new List<FakeStage>
                {
                    new FakeStage
                    {
                        StageIndex = 1,
                        StageName = "Orbital Stage",
                        StartMassKg = 6200,
                        EndMassKg = 1800,
                        MaxThrustNewton = 120000,
                        MinThrottle01 = 0.35,
                        IspVacSeconds = 345,
                        IspAtmSeconds = 270,
                        RemainingIgnitions = 2,
                        Engines = new List<FakeEngine>
                        {
                            new FakeEngine { EngineName = "AJE-45", MaxThrustNewton = 120000, MinThrottle01 = 0.35, IspVacSeconds = 345, IspAtmSeconds = 270, RemainingIgnitions = 2, RequiresUllage = true, PressureFed = false },
                        },
                    },
                },
            },
        };

        var context = new ReflectionKspGameContext(runtimeContext);
        var snapshot = context.CaptureGameState();

        Assert.Equal(123456.0, snapshot.UniversalTimeSeconds);
        Assert.Equal("Career-NASA", snapshot.SaveName);
        Assert.Equal(2, snapshot.CelestialBodies.Count);
        Assert.NotNull(snapshot.ActiveVessel);
        Assert.Equal("Mun Relay 1", snapshot.ActiveVessel!.Name);
        Assert.Equal("Kerbin", snapshot.ActiveVessel.BodyName);
        Assert.Equal(682000, snapshot.ActiveVessel.Orbit!.SemiMajorAxisMeters);
        Assert.Single(snapshot.ActiveVessel.Stages);
        Assert.Single(snapshot.ActiveVessel.Stages[0].Engines);
        Assert.True(snapshot.ActiveVessel.Stages[0].Engines[0].RequiresUllage);
    }

    [Fact]
    public void ReflectionWaypointCatalog_FindsWaypointIgnoringCase()
    {
        var catalog = new ReflectionWaypointCatalog(new object[]
        {
            new FakeWaypoint { BodyName = "Kerbin", Name = "North Pole", BiomeName = "Ice Caps", Latitude = 90.0, Longitude = 0.0, Altitude = 15.0 },
            new FakeWaypoint { BodyName = "Mun", Name = "East Crater", BiomeName = "Midlands", Latitude = 12.4, Longitude = 88.1, Altitude = 1200.0 },
        });

        var found = catalog.TryGetWaypoint("kerbin", "north pole", out var waypoint);

        Assert.True(found);
        Assert.Equal("Ice Caps", waypoint.BiomeName);
        Assert.Equal(90.0, waypoint.LatitudeDegrees);
    }

    [Fact]
    public void ReflectionManeuverNodeWriter_PassesNodeValuesThrough()
    {
        var runtimeWriter = new FakeNodeWriter();
        var writer = new ReflectionManeuverNodeWriter(runtimeWriter);

        writer.AddNode(new ManeuverNodeRequest
        {
            UniversalTimeSeconds = 5000,
            ProgradeMetersPerSecond = 120,
            NormalMetersPerSecond = -4,
            RadialMetersPerSecond = 2.5,
        });

        Assert.Equal(5000, runtimeWriter.UniversalTimeSeconds);
        Assert.Equal(120, runtimeWriter.ProgradeMetersPerSecond);
        Assert.Equal(-4, runtimeWriter.NormalMetersPerSecond);
        Assert.Equal(2.5, runtimeWriter.RadialMetersPerSecond);
    }

    [Fact]
    public void ReflectionRealFuelsAdapter_MapsStageAndEngineConstraints()
    {
        var runtimeProvider = new FakeRealFuelsProvider
        {
            CurrentStageMinimumThrottle01 = 0.42,
            Stages = new List<FakeStage>
            {
                new FakeStage
                {
                    StageIndex = 2,
                    StageName = "Transfer Stage",
                    StartMassKg = 4800,
                    EndMassKg = 2200,
                    MaxThrustNewton = 90000,
                    MinThrottle01 = 0.42,
                    IspVacSeconds = 360,
                    IspAtmSeconds = 120,
                    RemainingIgnitions = 1,
                    Engines = new List<FakeEngine>
                    {
                        new FakeEngine { EngineName = "RL10 Analog", MaxThrustNewton = 90000, MinThrottle01 = 0.42, IspVacSeconds = 360, IspAtmSeconds = 120, RemainingIgnitions = 1, RequiresUllage = true, PressureFed = false },
                    },
                },
            },
        };

        var adapter = new ReflectionRealFuelsAdapter(runtimeProvider);
        var stages = adapter.CaptureStages();

        Assert.Equal(0.42, adapter.GetCurrentStageMinimumThrottle01());
        Assert.Single(stages);
        Assert.Equal(1, stages[0].RemainingIgnitions);
        Assert.True(stages[0].Engines[0].RequiresUllage);
    }

    [Fact]
    public void ReflectionFarAdapter_CapturesAerodynamicState()
    {
        var adapter = new ReflectionFarAdapter(new FakeFarFlightData
        {
            DynamicPressure = 24500,
            Density = 0.41,
            Mach = 1.8,
            DragLossEstimate = 165,
        });

        var snapshot = adapter.CaptureAerodynamics();

        Assert.Equal(24500, snapshot.DynamicPressurePa);
        Assert.Equal(0.41, snapshot.AtmosphericDensityKgPerCubicMeter);
        Assert.Equal(165, adapter.EstimateDragLossesMetersPerSecond());
    }

    [Fact]
    public void ReflectionMechJebAdapter_UpdatesControllerSettings()
    {
        var controller = new FakeMechJebController();
        var adapter = new ReflectionMechJebAdapter(controller);

        adapter.SetAscentLimitQ(22000);
        adapter.SetAscentMaxAcceleration(27.5);
        adapter.EngageAscentAutopilot();

        var snapshot = adapter.CaptureAscentSettings();
        Assert.Equal(22000, snapshot.LimitQPa);
        Assert.Equal(27.5, snapshot.MaxAccelerationMetersPerSecondSquared);
        Assert.True(snapshot.AscentAutopilotEnabled);
        Assert.True(snapshot.SupportsSurfaceHopGuidance);
    }

    [Fact]
    public void MissionPlannerBootstrap_AllowsSurfaceWaypointObjectivesWithWaypointCatalog()
    {
        var context = new ReflectionKspGameContext(new FakeGameContext { UniversalTimeSeconds = 1, CurrentSaveName = "Career" });
        var waypoints = new ReflectionWaypointCatalog(new object[]
        {
            new FakeWaypoint { BodyName = "Mun", Name = "Biome Hop 1", BiomeName = "Midlands", Latitude = 12.0, Longitude = 30.0, Altitude = 500.0 },
        });
        var bootstrap = new MissionPlannerBootstrap(context, waypoints);

        var plan = bootstrap.BuildReversePlan(new MissionObjective
        {
            Name = "Mun Science Hop",
            TargetBody = "Mun",
            ObjectiveType = MissionObjectiveType.SurfaceWaypoint,
            SurfaceTarget = new SurfaceTarget
            {
                BodyName = "Mun",
                WaypointName = "Biome Hop 1",
            },
        });

        Assert.Equal("Mun Science Hop", plan.Objective.Name);
        Assert.Equal(MissionObjectiveType.SurfaceWaypoint, plan.Objective.ObjectiveType);
    }

    [Fact]
    public void AscentOptimizerBootstrap_ConfiguresMechJebFromAdapters()
    {
        var mechJeb = new ReflectionMechJebAdapter(new FakeMechJebController());
        var realFuels = new ReflectionRealFuelsAdapter(new FakeRealFuelsProvider { CurrentStageMinimumThrottle01 = 0.35 });
        var far = new ReflectionFarAdapter(new FakeFarFlightData { DragLossEstimate = 140 });
        var bootstrap = new AscentOptimizerBootstrap(mechJeb, realFuels, far);

        bootstrap.ConfigureAndEngage(
            new VehicleProfile { CraftName = "Surveyor" },
            new GuidanceRequest
            {
                Mode = GuidanceMode.SurfaceHop,
                SurfaceHopTarget = new SurfaceHopGuidanceTarget
                {
                    BodyName = "Kerbin",
                    DepartureName = "Woomerang",
                    DestinationName = "North Pole",
                },
            });

        var snapshot = mechJeb.CaptureAscentSettings();
        Assert.Equal(30.0, snapshot.MaxAccelerationMetersPerSecondSquared);
        Assert.Equal(25000.0, snapshot.LimitQPa);
        Assert.True(snapshot.AscentAutopilotEnabled);
    }

    [Fact]
    public void MissionPlannerBootstrap_PersistsMultipleMissionPrograms()
    {
        var context = new ReflectionKspGameContext(new FakeGameContext { UniversalTimeSeconds = 100, CurrentSaveName = "Career" });
        var store = new InMemoryMissionProgramStore();
        var bootstrap = new MissionPlannerBootstrap(context, missionStore: store);

        var missionA = new MissionProgram { MissionId = "mission-a", Name = "Mun Landing" };
        var missionB = new MissionProgram { MissionId = "mission-b", Name = "Minmus Orbiter" };

        bootstrap.SaveMissionProgram(missionA);
        bootstrap.SaveMissionProgram(missionB);

        var allMissions = bootstrap.GetMissionPrograms();
        Assert.Equal(2, allMissions.Count);
        Assert.Contains(allMissions, mission => mission.MissionId == "mission-a");
        Assert.Contains(allMissions, mission => mission.MissionId == "mission-b");

        var deleted = bootstrap.DeleteMissionProgram("mission-a");
        Assert.True(deleted);
        Assert.Single(bootstrap.GetMissionPrograms());
    }

    [Fact]
    public void MissionPlannerBootstrap_IncludesGravityAssistCandidatesInTransitionAssessments()
    {
        var context = new ReflectionKspGameContext(new FakeGameContext { UniversalTimeSeconds = 43210, CurrentSaveName = "Career" });
        var gravityScout = new FixedGravityAssistScout();
        var bootstrap = new MissionPlannerBootstrap(context, gravityAssistScout: gravityScout);

        var missionProgram = new MissionProgram
        {
            MissionId = "interplanetary-1",
            Name = "Duna Relay",
            Stages = new[]
            {
                new MissionStage
                {
                    StageNumber = 1,
                    Name = "Transfer",
                    Goals = new[]
                    {
                        new MissionGoal { GoalId = "goal-kerbin-orbit", Name = "Kerbin Parking Orbit", GoalType = MissionGoalType.Orbit, TargetBody = "Kerbin" },
                        new MissionGoal { GoalId = "goal-duna-orbit", Name = "Duna Capture Orbit", GoalType = MissionGoalType.Orbit, TargetBody = "Duna" },
                    },
                },
            },
        };

        var plan = bootstrap.BuildReversePlan(missionProgram);

        Assert.Single(plan.TransitionAssessments);
        Assert.Single(plan.TransitionAssessments[0].GravityAssistCandidates);
        Assert.Equal("Eve", plan.TransitionAssessments[0].GravityAssistCandidates[0].AssistBodyName);
        Assert.Single(plan.Legs);
        Assert.Single(plan.Legs[0].GravityAssistCandidates);
    }

    [Fact]
    public void MissionPlannerBootstrap_EnsuresParkingOrbitGoalsAroundLandingGoal()
    {
        var context = new ReflectionKspGameContext(new FakeGameContext { UniversalTimeSeconds = 1, CurrentSaveName = "Career" });
        var bootstrap = new MissionPlannerBootstrap(context);
        var missionProgram = new MissionProgram
        {
            MissionId = "mun-surface-mission",
            Name = "Mun Site Landing",
            Stages = new[]
            {
                new MissionStage
                {
                    StageNumber = 1,
                    Name = "Surface Mission",
                    Goals = new[]
                    {
                        new MissionGoal
                        {
                            GoalId = "landing-goal",
                            Name = "Land at Site Alpha",
                            GoalType = MissionGoalType.Landing,
                            TargetBody = "Mun",
                            SurfaceTarget = new SurfaceTarget { BodyName = "Mun", LatitudeDegrees = 12, LongitudeDegrees = 45 },
                        },
                    },
                },
            },
        };

        var augmented = bootstrap.EnsureParkingOrbitGoals(
            missionProgram,
            new ParkingOrbitTemplate
            {
                DefaultPeriapsisAltitudeMeters = 15000,
                DefaultApoapsisAltitudeMeters = 18000,
                DefaultInclinationRadians = 0.0,
            },
            addBeforeLanding: true,
            addAfterLanding: true);

        Assert.Single(augmented.Stages);
        Assert.Equal(3, augmented.Stages[0].Goals.Count);
        Assert.Equal(MissionGoalType.ParkingOrbit, augmented.Stages[0].Goals[0].GoalType);
        Assert.Equal(MissionGoalType.Landing, augmented.Stages[0].Goals[1].GoalType);
        Assert.Equal(MissionGoalType.ParkingOrbit, augmented.Stages[0].Goals[2].GoalType);
    }

    private sealed class FakeGameContext
    {
        public double UniversalTimeSeconds { get; set; }
        public string CurrentSaveName { get; set; } = string.Empty;
        public List<FakeBody> CelestialBodies { get; set; } = new List<FakeBody>();
        public FakeVessel? ActiveVessel { get; set; }
    }

    private sealed class FakeBody
    {
        public string Name { get; set; } = string.Empty;
        public double gravParameter { get; set; }
        public double Radius { get; set; }
        public double atmosphereDepth { get; set; }
        public bool atmosphere { get; set; }
    }

    private sealed class FakeVessel
    {
        public string VesselName { get; set; } = string.Empty;
        public string MainBodyName { get; set; } = string.Empty;
        public double Mass { get; set; }
        public double DryMass { get; set; }
        public double altitude { get; set; }
        public double srfSpeed { get; set; }
        public double obt_speed { get; set; }
        public FakeOrbit? Orbit { get; set; }
        public List<FakeStage> Stages { get; set; } = new List<FakeStage>();
    }

    private sealed class FakeOrbit
    {
        public string BodyName { get; set; } = string.Empty;
        public double inclination { get; set; }
        public double eccentricity { get; set; }
        public double semiMajorAxis { get; set; }
        public double PeA { get; set; }
        public double ApA { get; set; }
        public double LAN { get; set; }
        public double argumentOfPeriapsis { get; set; }
        public double meanAnomalyAtEpoch { get; set; }
        public double epoch { get; set; }
    }

    private sealed class FakeStage
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
        public List<FakeEngine> Engines { get; set; } = new List<FakeEngine>();
    }

    private sealed class FakeEngine
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

    private sealed class FakeWaypoint
    {
        public string BodyName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BiomeName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
    }

    private sealed class FakeNodeWriter
    {
        public double UniversalTimeSeconds { get; private set; }
        public double ProgradeMetersPerSecond { get; private set; }
        public double NormalMetersPerSecond { get; private set; }
        public double RadialMetersPerSecond { get; private set; }

        public void AddNode(double universalTimeSeconds, double prograde, double normal, double radial)
        {
            UniversalTimeSeconds = universalTimeSeconds;
            ProgradeMetersPerSecond = prograde;
            NormalMetersPerSecond = normal;
            RadialMetersPerSecond = radial;
        }
    }

    private sealed class FakeRealFuelsProvider
    {
        public double CurrentStageMinimumThrottle01 { get; set; }
        public List<FakeStage> Stages { get; set; } = new List<FakeStage>();
    }

    private sealed class FakeFarFlightData
    {
        public double DynamicPressure { get; set; }
        public double Density { get; set; }
        public double Mach { get; set; }
        public double DragLossEstimate { get; set; }
    }

    private sealed class FakeMechJebController
    {
        public double LimitQPa { get; set; }
        public double MaxAccelerationMetersPerSecondSquared { get; set; }
        public bool AscentAutopilotEnabled { get; set; }
        public bool SupportsSurfaceHopGuidance { get; set; } = true;

        public void EngageAscentAutopilot()
        {
            AscentAutopilotEnabled = true;
        }
    }

    private sealed class FixedGravityAssistScout : IGravityAssistScout
    {
        public IReadOnlyList<GravityAssistOption> FindCandidates(
            MissionGoal fromGoal,
            MissionGoal toGoal,
            double universalTimeSeconds)
        {
            _ = fromGoal;
            _ = toGoal;
            return new[]
            {
                new GravityAssistOption
                {
                    AssistBodyName = "Eve",
                    EncounterWindowStartUtSeconds = universalTimeSeconds + 10000,
                    EncounterWindowEndUtSeconds = universalTimeSeconds + 16000,
                    EstimatedDeltaVSavingsMetersPerSecond = 420,
                    EstimatedFlightTimeChangeSeconds = -3 * 24 * 60 * 60,
                    Notes = "Coarse pre-screened assist candidate",
                },
            };
        }
    }
}