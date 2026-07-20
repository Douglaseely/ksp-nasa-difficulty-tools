using System;
using System.Collections.Generic;
using System.Linq;
using KspIntegration.Mission;
using KspIntegration.Models;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Plugin;

public sealed class MissionPlannerController
{
    private static readonly DateTimeOffset UnixEpochUtc = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly MissionPlannerBootstrap _bootstrap;
    private readonly IKspGameContext _gameContext;
    private readonly IManeuverNodeWriter? _maneuverNodeWriter;
    private readonly bool _waypointCatalogAvailable;

    public MissionPlannerController(
        MissionPlannerBootstrap bootstrap,
        IKspGameContext gameContext,
        bool waypointCatalogAvailable,
        IManeuverNodeWriter? maneuverNodeWriter = null)
    {
        _bootstrap = bootstrap ?? throw new ArgumentNullException(nameof(bootstrap));
        _gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        _maneuverNodeWriter = maneuverNodeWriter;
        _waypointCatalogAvailable = waypointCatalogAvailable;
    }

    public MissionPlannerViewState CreateInitialViewState()
    {
        return new MissionPlannerViewState
        {
            Diagnostics = CaptureDiagnostics(),
        };
    }

    public IReadOnlyList<MissionProgram> LoadMissionPrograms()
    {
        return _bootstrap.GetMissionPrograms();
    }

    public bool TryLoadMissionProgram(string missionId, out MissionProgram missionProgram)
    {
        return _bootstrap.TryGetMissionProgram(missionId, out missionProgram!);
    }

    public MissionProgram? GetActiveMissionProgram()
    {
        if (string.IsNullOrWhiteSpace(MissionPlannerSessionState.ActiveMissionId))
        {
            return null;
        }

        return TryLoadMissionProgram(MissionPlannerSessionState.ActiveMissionId, out var missionProgram)
            ? missionProgram
            : null;
    }

    public void SetActiveMission(string missionId)
    {
        MissionPlannerSessionState.ActiveMissionId = missionId;
    }

    public MissionProgram CreateMissionProgram(string name)
    {
        return new MissionProgram
        {
            Name = string.IsNullOrWhiteSpace(name) ? "New Mission" : name.Trim(),
            Stages = new[]
            {
                new MissionStage
                {
                    StageNumber = 1,
                    Name = "Stage 1",
                    Goals = Array.Empty<MissionGoal>(),
                },
            },
        };
    }

    public MissionProgram DuplicateMissionProgram(MissionProgram source, string? duplicateName = null)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var clone = MissionProgramCloner.Clone(source);
        clone.MissionId = Guid.NewGuid().ToString("N");
        clone.Name = string.IsNullOrWhiteSpace(duplicateName) ? source.Name + " Copy" : duplicateName!.Trim();
        clone.CreatedUtc = DateTimeOffset.UtcNow;
        clone.UpdatedUtc = clone.CreatedUtc;
        return clone;
    }

    public MissionProgram SaveMissionProgram(MissionProgram missionProgram)
    {
        ValidateMissionProgramForSavingOrThrow(missionProgram);
        return _bootstrap.SaveMissionProgram(missionProgram);
    }

    public bool DeleteMissionProgram(string missionId)
    {
        return _bootstrap.DeleteMissionProgram(missionId);
    }

    public MissionPlan GeneratePlan(MissionProgram missionProgram)
    {
        ValidateMissionProgramForPlanningOrThrow(missionProgram);
        return _bootstrap.BuildReversePlan(missionProgram);
    }

    public MissionProgram AutoInsertParkingOrbits(
        MissionProgram missionProgram,
        ParkingOrbitTemplate template,
        bool addBeforeLanding,
        bool addAfterLanding)
    {
        return _bootstrap.EnsureParkingOrbitGoals(missionProgram, template, addBeforeLanding, addAfterLanding);
    }

    public IReadOnlyList<string> GetAvailableBodyNames()
    {
        var names = _gameContext
            .CaptureGameState()
            .CelestialBodies
            .Select(body => body.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0)
        {
            names.Add("Kerbin");
        }

        return names;
    }

    public int ExportPlannedManeuvers(MissionPlan missionPlan)
    {
        if (missionPlan is null)
        {
            throw new ArgumentNullException(nameof(missionPlan));
        }

        if (_maneuverNodeWriter is null)
        {
            throw new InvalidOperationException("Maneuver node export is not available in the current runtime.");
        }

        var exported = 0;
        foreach (var maneuver in missionPlan.Maneuvers)
        {
            _maneuverNodeWriter.AddNode(new ManeuverNodeRequest
            {
                UniversalTimeSeconds = (maneuver.PlannedTimeUtc - UnixEpochUtc).TotalSeconds,
                ProgradeMetersPerSecond = maneuver.DeltaVMetersPerSecond,
                NormalMetersPerSecond = 0.0,
                RadialMetersPerSecond = 0.0,
            });
            exported++;
        }

        return exported;
    }

    public IReadOnlyList<string> ValidateMissionProgram(MissionProgram missionProgram)
    {
        return MissionProgramValidation.ValidateForSaving(missionProgram);
    }

    public IReadOnlyList<string> ValidateMissionProgramForPlanning(MissionProgram missionProgram)
    {
        return MissionProgramValidation.ValidateForPlanning(missionProgram);
    }

    public MissionPlannerDiagnosticsSnapshot CaptureDiagnostics()
    {
        var gameState = _gameContext.CaptureGameState();
        return new MissionPlannerDiagnosticsSnapshot
        {
            CurrentSaveName = _gameContext.CurrentSaveName,
            UniversalTimeSeconds = _gameContext.UniversalTimeSeconds,
            ActiveBodyName = gameState.ActiveVessel?.BodyName ?? string.Empty,
            WaypointCatalogAvailable = _waypointCatalogAvailable,
            ManeuverNodeExportAvailable = _maneuverNodeWriter is not null,
        };
    }

    private void ValidateMissionProgramForSavingOrThrow(MissionProgram missionProgram)
    {
        var messages = ValidateMissionProgram(missionProgram);
        if (messages.Count > 0)
        {
            throw new InvalidOperationException(string.Join(Environment.NewLine, messages));
        }
    }

    private void ValidateMissionProgramForPlanningOrThrow(MissionProgram missionProgram)
    {
        var messages = ValidateMissionProgramForPlanning(missionProgram);
        if (messages.Count > 0)
        {
            throw new InvalidOperationException(string.Join(Environment.NewLine, messages));
        }
    }
}