#if KSP_RUNTIME
using System;
using System.Collections.Generic;
using KspMissionPlanner.Planning;
using UnityEngine;

namespace KspMissionPlanner.Plugin;

public sealed class MissionPlannerWindow
{
    private readonly MissionPlannerController _controller;
    private readonly MissionPlannerWindowState _windowState = new MissionPlannerWindowState();

    public MissionPlannerWindow(MissionPlannerController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }

    public void Draw(MissionPlannerViewState viewState)
    {
        if (viewState.CurrentMissionProgram is not null)
        {
            MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
        }

        if (viewState.CurrentMissionProgram is null)
        {
            if (MissionPlannerSessionState.WorkingMissionProgram is not null)
            {
                var draft = MissionPlannerSessionState.WorkingMissionProgram;
                viewState.CurrentMissionProgram = draft;
                viewState.SelectedMissionId = draft.MissionId;
                viewState.ActiveMissionId = draft.MissionId;
            }
            else
            {
                var activeMission = _controller.GetActiveMissionProgram();
                if (activeMission is not null)
                {
                    viewState.CurrentMissionProgram = activeMission;
                    viewState.SelectedMissionId = activeMission.MissionId;
                    viewState.ActiveMissionId = activeMission.MissionId;
                }
                else
                {
                    var missions = _controller.LoadMissionPrograms();
                    if (missions.Count > 0)
                    {
                        viewState.CurrentMissionProgram = missions[0];
                        viewState.SelectedMissionId = missions[0].MissionId;
                        viewState.ActiveMissionId = missions[0].MissionId;
                        _controller.SetActiveMission(missions[0].MissionId);
                    }
                }
            }
        }

        DrawTabs(viewState);

        if (!string.IsNullOrWhiteSpace(_windowState.StatusMessage))
        {
            GUILayout.Label(_windowState.StatusMessage);
        }

        switch (viewState.SelectedTab)
        {
            case MissionPlannerTab.Missions:
                DrawMissionsTab(viewState);
                break;
            case MissionPlannerTab.Stages:
                DrawStagesTab(viewState);
                break;
            case MissionPlannerTab.Planning:
                DrawPlanningTab(viewState);
                break;
            case MissionPlannerTab.Actions:
                DrawActionsTab(viewState);
                break;
            case MissionPlannerTab.Diagnostics:
                DrawDiagnosticsTab(viewState);
                break;
        }
    }

    private void DrawTabs(MissionPlannerViewState viewState)
    {
        GUILayout.BeginHorizontal();
        DrawTabButton(viewState, MissionPlannerTab.Missions, "Missions");
        DrawTabButton(viewState, MissionPlannerTab.Stages, "Stages");
        DrawTabButton(viewState, MissionPlannerTab.Planning, "Planning");
        DrawTabButton(viewState, MissionPlannerTab.Actions, "Actions");
        DrawTabButton(viewState, MissionPlannerTab.Diagnostics, "Diagnostics");
        GUILayout.EndHorizontal();
    }

    private static void DrawTabButton(MissionPlannerViewState viewState, MissionPlannerTab tab, string label)
    {
        if (GUILayout.Button(label))
        {
            MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
            viewState.SelectedTab = tab;
        }
    }

    private void DrawMissionsTab(MissionPlannerViewState viewState)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("New mission:", GUILayout.Width(90));
        _windowState.NewMissionName = GUILayout.TextField(_windowState.NewMissionName);
        if (GUILayout.Button("Create", GUILayout.Width(80)))
        {
            ExecuteAndReport(() =>
            {
                viewState.CurrentMissionProgram = _controller.CreateMissionProgram(_windowState.NewMissionName);
                viewState.CurrentMissionProgram = _controller.SaveMissionProgram(viewState.CurrentMissionProgram);
                viewState.SelectedMissionId = viewState.CurrentMissionProgram.MissionId;
                viewState.ActiveMissionId = viewState.CurrentMissionProgram.MissionId;
                _controller.SetActiveMission(viewState.ActiveMissionId);
                MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
                viewState.IsDirty = false;
            }, "Created new mission program.");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(8f);
        _windowState.MissionScrollPosition = GUILayout.BeginScrollView(_windowState.MissionScrollPosition, GUILayout.Height(280));
        foreach (var mission in _controller.LoadMissionPrograms())
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(mission.Name);
            if (GUILayout.Button("Load", GUILayout.Width(70)))
            {
                viewState.CurrentMissionProgram = mission;
                viewState.SelectedMissionId = mission.MissionId;
                viewState.ActiveMissionId = mission.MissionId;
                _controller.SetActiveMission(mission.MissionId);
                viewState.CurrentPlan = null;
                viewState.ValidationMessages = Array.Empty<string>();
                MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
                _windowState.StatusMessage = "Loaded mission program.";
            }

            if (GUILayout.Button(viewState.ActiveMissionId == mission.MissionId ? "Active" : "Set Active", GUILayout.Width(90)))
            {
                viewState.CurrentMissionProgram = mission;
                viewState.SelectedMissionId = mission.MissionId;
                viewState.ActiveMissionId = mission.MissionId;
                _controller.SetActiveMission(mission.MissionId);
                MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
                _windowState.StatusMessage = "Active mission selected.";
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        if (viewState.CurrentMissionProgram is null)
        {
            return;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Duplicate"))
        {
            viewState.CurrentMissionProgram = _controller.DuplicateMissionProgram(viewState.CurrentMissionProgram);
            viewState.SelectedMissionId = viewState.CurrentMissionProgram.MissionId;
                viewState.ActiveMissionId = viewState.CurrentMissionProgram.MissionId;
                _controller.SetActiveMission(viewState.ActiveMissionId);
                MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
            viewState.IsDirty = true;
            _windowState.StatusMessage = "Duplicated current mission program.";
        }

        if (GUILayout.Button("Save"))
        {
            ExecuteAndReport(() =>
            {
                viewState.CurrentMissionProgram = _controller.SaveMissionProgram(viewState.CurrentMissionProgram);
                viewState.IsDirty = false;
            }, "Mission program saved.");
        }

        if (GUILayout.Button("Delete"))
        {
            if (_controller.DeleteMissionProgram(viewState.CurrentMissionProgram.MissionId))
            {
                viewState.CurrentMissionProgram = null;
                viewState.CurrentPlan = null;
                viewState.SelectedMissionId = string.Empty;
                viewState.ActiveMissionId = string.Empty;
                _controller.SetActiveMission(string.Empty);
                MissionPlannerSessionState.WorkingMissionProgram = null;
                _windowState.StatusMessage = "Mission program deleted.";
            }
        }
        GUILayout.EndHorizontal();
    }

    private void DrawStagesTab(MissionPlannerViewState viewState)
    {
        if (viewState.CurrentMissionProgram is null)
        {
            GUILayout.Label("Create or load a mission program first.");
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mission name:", GUILayout.Width(90));
        viewState.CurrentMissionProgram.Name = GUILayout.TextField(viewState.CurrentMissionProgram.Name);
        GUILayout.EndHorizontal();

        _windowState.EditorScrollPosition = GUILayout.BeginScrollView(_windowState.EditorScrollPosition, GUILayout.Height(340));
        var stageIndex = 0;
        foreach (var stage in viewState.CurrentMissionProgram.Stages)
        {
            DrawStageEditor(viewState, stage, stageIndex);
            stageIndex++;
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Stage"))
        {
            AddStage(viewState.CurrentMissionProgram);
            viewState.IsDirty = true;
            MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
        }

        if (GUILayout.Button("Add Orbit Goal") && viewState.CurrentMissionProgram.Stages.Count > 0)
        {
            AddGoal(viewState.CurrentMissionProgram.Stages[viewState.CurrentMissionProgram.Stages.Count - 1], MissionGoalType.Orbit);
            viewState.IsDirty = true;
            MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
        }

        if (GUILayout.Button("Add Surface Goal") && viewState.CurrentMissionProgram.Stages.Count > 0)
        {
            AddGoal(viewState.CurrentMissionProgram.Stages[viewState.CurrentMissionProgram.Stages.Count - 1], MissionGoalType.Landing);
            viewState.IsDirty = true;
            MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
        }
        GUILayout.EndHorizontal();
    }

    private void DrawStageEditor(MissionPlannerViewState viewState, MissionStage stage, int stageIndex)
    {
        if (viewState.CurrentMissionProgram is not null && StageHasLandingWithoutAdjacentParking(stage))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Landing stage missing parking orbit coverage.", GUILayout.Width(310));
            if (GUILayout.Button("Generate Parking Orbit", GUILayout.Width(210)))
            {
                ExecuteAndReport(() =>
                {
                    viewState.CurrentMissionProgram = _controller.AutoInsertParkingOrbits(
                        viewState.CurrentMissionProgram,
                        new ParkingOrbitTemplate
                        {
                            DefaultPeriapsisAltitudeMeters = ParseDouble(_windowState.ParkingPeriapsisText, 15000.0),
                            DefaultApoapsisAltitudeMeters = ParseDouble(_windowState.ParkingApoapsisText, 18000.0),
                            DefaultInclinationRadians = ParseDouble(_windowState.ParkingInclinationText, 0.0),
                        },
                        addBeforeLanding: true,
                        addAfterLanding: true);

                    MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
                    viewState.IsDirty = true;
                }, $"Generated parking orbits for stage {stageIndex + 1}.");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Default Pe/Ap/Inc:", GUILayout.Width(120));
            _windowState.ParkingPeriapsisText = GUILayout.TextField(_windowState.ParkingPeriapsisText, GUILayout.Width(80));
            _windowState.ParkingApoapsisText = GUILayout.TextField(_windowState.ParkingApoapsisText, GUILayout.Width(80));
            _windowState.ParkingInclinationText = GUILayout.TextField(_windowState.ParkingInclinationText, GUILayout.Width(80));
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Stage {stage.StageNumber}", GUILayout.Width(70));
        stage.Name = GUILayout.TextField(stage.Name);
        GUILayout.EndHorizontal();

        foreach (var goal in stage.Goals)
        {
            DrawGoalEditor(viewState, goal);
        }
        GUILayout.EndVertical();
    }

    private void DrawGoalEditor(MissionPlannerViewState viewState, MissionGoal goal)
    {
        var priorGoalName = goal.Name;
        var priorBody = goal.TargetBody;
        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        goal.Name = LabeledTextField("Goal", goal.Name, 60f);
        DrawBodyDropdown(goal);
        if (GUILayout.Button(goal.GoalType.ToString(), GUILayout.Width(150)))
        {
            goal.GoalType = GetNextGoalType(goal.GoalType);
            viewState.IsDirty = true;
        }
        GUILayout.EndHorizontal();

        if (goal.GoalType == MissionGoalType.Orbit || goal.GoalType == MissionGoalType.ParkingOrbit || goal.GoalType == MissionGoalType.Flyby)
        {
            goal.OrbitTarget ??= new OrbitTarget { BodyName = goal.TargetBody };
            goal.OrbitTarget.BodyName = goal.TargetBody;
            goal.OrbitTarget.PeriapsisAltitudeMeters = LabeledDoubleField("Pe m", goal.OrbitTarget.PeriapsisAltitudeMeters, 60f);
            goal.OrbitTarget.ApoapsisAltitudeMeters = LabeledDoubleField("Ap m", goal.OrbitTarget.ApoapsisAltitudeMeters, 60f);
            goal.OrbitTarget.InclinationRadians = LabeledDoubleField("Inc rad", goal.OrbitTarget.InclinationRadians, 70f);
        }
        else
        {
            goal.SurfaceTarget ??= new SurfaceTarget { BodyName = goal.TargetBody };
            goal.SurfaceTarget.BodyName = goal.TargetBody;
            goal.SurfaceTarget.WaypointName = LabeledTextField("Waypoint", goal.SurfaceTarget.WaypointName, 70f);
            goal.SurfaceTarget.LatitudeDegrees = LabeledDoubleField("Lat", goal.SurfaceTarget.LatitudeDegrees, 45f);
            goal.SurfaceTarget.LongitudeDegrees = LabeledDoubleField("Lon", goal.SurfaceTarget.LongitudeDegrees, 45f);
            goal.SurfaceTarget.AltitudeMeters = LabeledDoubleField("Alt", goal.SurfaceTarget.AltitudeMeters, 45f);
        }

        GUILayout.EndVertical();
        if (!string.Equals(priorGoalName, goal.Name, StringComparison.Ordinal) ||
            !string.Equals(priorBody, goal.TargetBody, StringComparison.Ordinal))
        {
            viewState.IsDirty = true;
        }

        MissionPlannerSessionState.WorkingMissionProgram = viewState.CurrentMissionProgram;
    }

    private void DrawPlanningTab(MissionPlannerViewState viewState)
    {
        if (viewState.CurrentPlan is null)
        {
            GUILayout.Label("Generate a plan to view leg estimates and maneuvers.");
            return;
        }

        GUILayout.Label($"Active mission: {viewState.CurrentMissionProgram?.Name}");

        _windowState.PlanningScrollPosition = GUILayout.BeginScrollView(_windowState.PlanningScrollPosition, GUILayout.Height(360));
        for (var legIndex = 0; legIndex < viewState.CurrentPlan.Legs.Count; legIndex++)
        {
            var leg = viewState.CurrentPlan.Legs[legIndex];
            var legColor = GetStageColor(legIndex);
            var legStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = legColor },
                fontStyle = FontStyle.Bold,
            };

            GUILayout.BeginVertical("box");
            GUILayout.Label($"Leg {legIndex + 1}: {leg.Description}", legStyle);
            GUILayout.Label($"Type: {leg.LegType}", legStyle);
            GUILayout.Label($"Delta-v: {leg.EstimatedDeltaVMetersPerSecond:0.##} m/s", legStyle);
            GUILayout.Label($"Coast: {leg.EstimatedCoastTimeSeconds:0.##} s", legStyle);
            GUILayout.EndVertical();
        }

        for (var maneuverIndex = 0; maneuverIndex < viewState.CurrentPlan.Maneuvers.Count; maneuverIndex++)
        {
            var maneuver = viewState.CurrentPlan.Maneuvers[maneuverIndex];
            var maneuverColor = GetManeuverColor(maneuverIndex);
            var maneuverStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = maneuverColor },
                fontStyle = FontStyle.Bold,
            };

            GUILayout.BeginVertical("box");
            GUILayout.Label($"Maneuver {maneuverIndex + 1}: {maneuver.Description}", maneuverStyle);
            GUILayout.Label($"Burn: {maneuver.DeltaVMetersPerSecond:0.##} m/s for {maneuver.BurnDurationSeconds:0.##} s", maneuverStyle);
            GUILayout.Label($"Planned time: {maneuver.PlannedTimeUtc:u}", maneuverStyle);
            GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();
    }

    private void DrawActionsTab(MissionPlannerViewState viewState)
    {
        if (viewState.CurrentMissionProgram is null)
        {
            GUILayout.Label("Create or load a mission program first.");
            return;
        }

        viewState.ValidationMessages = _controller.ValidateMissionProgramForPlanning(viewState.CurrentMissionProgram);
        foreach (var message in viewState.ValidationMessages)
        {
            GUILayout.Label(message);
        }

        if (GUILayout.Button("Generate Plan"))
        {
            ExecuteAndReport(() =>
            {
                viewState.CurrentPlan = _controller.GeneratePlan(viewState.CurrentMissionProgram);
                viewState.SelectedTab = MissionPlannerTab.Planning;
            }, "Plan generated.");
        }

        if (GUILayout.Button("Export Maneuver Nodes"))
        {
            ExecuteAndReport(() =>
            {
                var count = _controller.ExportPlannedManeuvers(viewState.CurrentPlan ?? throw new InvalidOperationException("No plan available."));
                _windowState.StatusMessage = $"Exported {count} maneuver nodes.";
            }, "Maneuver export completed.");
        }
    }

    private void DrawDiagnosticsTab(MissionPlannerViewState viewState)
    {
        viewState.Diagnostics = _controller.CaptureDiagnostics();
        GUILayout.Label($"Save: {viewState.Diagnostics.CurrentSaveName}");
        GUILayout.Label($"UT: {viewState.Diagnostics.UniversalTimeSeconds:0.##}");
        GUILayout.Label($"Active body: {viewState.Diagnostics.ActiveBodyName}");
        GUILayout.Label($"Waypoint catalog: {viewState.Diagnostics.WaypointCatalogAvailable}");
        GUILayout.Label($"Node export: {viewState.Diagnostics.ManeuverNodeExportAvailable}");
    }

    private void ExecuteAndReport(Action action, string successMessage)
    {
        try
        {
            action();
            _windowState.StatusMessage = successMessage;
        }
        catch (Exception ex)
        {
            _windowState.StatusMessage = ex.Message;
        }
    }

    private static string LabeledTextField(string label, string value, float width)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(width));
        var result = GUILayout.TextField(value ?? string.Empty);
        GUILayout.EndHorizontal();
        return result;
    }

    private void DrawBodyDropdown(MissionGoal goal)
    {
        GUILayout.Label("Body", GUILayout.Width(45));

        if (string.IsNullOrWhiteSpace(goal.TargetBody))
        {
            goal.TargetBody = "Kerbin";
        }

        if (GUILayout.Button(goal.TargetBody + " v", GUILayout.Width(170)))
        {
            _windowState.ExpandedBodyGoalId = _windowState.ExpandedBodyGoalId == goal.GoalId ? string.Empty : goal.GoalId;
        }

        if (_windowState.ExpandedBodyGoalId != goal.GoalId)
        {
            return;
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(106f);
        GUILayout.BeginVertical("box", GUILayout.Width(170));
        foreach (var bodyName in _controller.GetAvailableBodyNames())
        {
            if (!GUILayout.Button(bodyName))
            {
                continue;
            }

            goal.TargetBody = bodyName;
            if (goal.OrbitTarget is not null)
            {
                goal.OrbitTarget.BodyName = bodyName;
            }

            if (goal.SurfaceTarget is not null)
            {
                goal.SurfaceTarget.BodyName = bodyName;
            }

            _windowState.ExpandedBodyGoalId = string.Empty;
            break;
        }
        GUILayout.EndVertical();
    }

    private static double LabeledDoubleField(string label, double value, float width)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(width));
        var text = GUILayout.TextField(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        GUILayout.EndHorizontal();
        return ParseDouble(text, value);
    }

    private static double ParseDouble(string text, double fallback)
    {
        return double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fallback;
    }

    private static MissionGoalType GetNextGoalType(MissionGoalType current)
    {
        var values = (MissionGoalType[])Enum.GetValues(typeof(MissionGoalType));
        var index = Array.IndexOf(values, current);
        return values[(index + 1) % values.Length];
    }

    private static void AddStage(MissionProgram missionProgram)
    {
        var stages = new List<MissionStage>(missionProgram.Stages)
        {
            new MissionStage
            {
                StageNumber = missionProgram.Stages.Count + 1,
                Name = $"Stage {missionProgram.Stages.Count + 1}",
                Goals = Array.Empty<MissionGoal>(),
            },
        };

        missionProgram.Stages = stages;
    }

    private static void AddGoal(MissionStage stage, MissionGoalType goalType)
    {
        var goals = new List<MissionGoal>(stage.Goals)
        {
            CreateDefaultGoal(goalType),
        };

        stage.Goals = goals;
    }

    private static MissionGoal CreateDefaultGoal(MissionGoalType goalType)
    {
        return new MissionGoal
        {
            Name = goalType + " Goal",
            GoalType = goalType,
            TargetBody = "Kerbin",
            OrbitTarget = goalType == MissionGoalType.Orbit || goalType == MissionGoalType.ParkingOrbit || goalType == MissionGoalType.Flyby
                ? new OrbitTarget { BodyName = "Kerbin", PeriapsisAltitudeMeters = 80000, ApoapsisAltitudeMeters = 80000 }
                : null,
            SurfaceTarget = goalType == MissionGoalType.Landing || goalType == MissionGoalType.SurfaceWaypoint || goalType == MissionGoalType.SurfaceHop
                ? new SurfaceTarget { BodyName = "Kerbin", WaypointName = string.Empty }
                : null,
        };
    }

    private static bool StageHasLandingWithoutAdjacentParking(MissionStage stage)
    {
        for (var index = 0; index < stage.Goals.Count; index++)
        {
            var goal = stage.Goals[index];
            if (goal.GoalType != MissionGoalType.Landing && goal.GoalType != MissionGoalType.SurfaceWaypoint)
            {
                continue;
            }

            var hasBefore = index > 0 &&
                            stage.Goals[index - 1].GoalType == MissionGoalType.ParkingOrbit &&
                            string.Equals(stage.Goals[index - 1].TargetBody, goal.TargetBody, StringComparison.OrdinalIgnoreCase);
            var hasAfter = index + 1 < stage.Goals.Count &&
                           stage.Goals[index + 1].GoalType == MissionGoalType.ParkingOrbit &&
                           string.Equals(stage.Goals[index + 1].TargetBody, goal.TargetBody, StringComparison.OrdinalIgnoreCase);

            if (!hasBefore || !hasAfter)
            {
                return true;
            }
        }

        return false;
    }

    private static Color GetStageColor(int index)
    {
        Color[] colors =
        {
            new Color(0.94f, 0.28f, 0.26f, 1f),
            new Color(0.18f, 0.78f, 0.43f, 1f),
            new Color(0.23f, 0.56f, 0.95f, 1f),
            new Color(0.95f, 0.76f, 0.22f, 1f),
            new Color(0.88f, 0.42f, 0.94f, 1f),
            new Color(0.20f, 0.84f, 0.84f, 1f),
        };
        return colors[Mathf.Abs(index) % colors.Length];
    }

    private static Color GetManeuverColor(int index)
    {
        Color[] colors =
        {
            new Color(1f, 1f, 1f, 1f),
            new Color(1f, 0.86f, 0.36f, 1f),
            new Color(1f, 0.48f, 0.48f, 1f),
            new Color(0.55f, 0.90f, 1f, 1f),
        };
        return colors[Mathf.Abs(index) % colors.Length];
    }
}
#endif