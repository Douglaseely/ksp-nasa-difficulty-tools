#if KSP_RUNTIME
using System;
using KspAscentOptimizer.Integrations;
using KspAscentOptimizer.Optimization;
using UnityEngine;

namespace KspAscentOptimizer.Plugin;

public sealed class AscentOptimizerWindow
{
    private readonly AscentOptimizerController _controller;
    private readonly AscentOptimizerWindowState _windowState = new AscentOptimizerWindowState();

    public AscentOptimizerWindow(AscentOptimizerController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }

    public void Draw(AscentOptimizerViewState viewState)
    {
        viewState.CurrentVehicle ??= _controller.BuildVehicleProfile("Active Vessel");
        DrawTabs(viewState);

        if (!string.IsNullOrWhiteSpace(_windowState.StatusMessage))
        {
            GUILayout.Label(_windowState.StatusMessage);
        }

        switch (viewState.SelectedTab)
        {
            case AscentOptimizerTab.Vehicle:
                DrawVehicleTab(viewState);
                break;
            case AscentOptimizerTab.Guidance:
                DrawGuidanceTab(viewState);
                break;
            case AscentOptimizerTab.Constraints:
                DrawConstraintsTab(viewState);
                break;
            case AscentOptimizerTab.Diagnostics:
                DrawDiagnosticsTab(viewState);
                break;
        }
    }

    private void DrawTabs(AscentOptimizerViewState viewState)
    {
        GUILayout.BeginHorizontal();
        DrawTabButton(viewState, AscentOptimizerTab.Vehicle, "Vehicle");
        DrawTabButton(viewState, AscentOptimizerTab.Guidance, "Guidance");
        DrawTabButton(viewState, AscentOptimizerTab.Constraints, "Constraints");
        DrawTabButton(viewState, AscentOptimizerTab.Diagnostics, "Diagnostics");
        GUILayout.EndHorizontal();
    }

    private static void DrawTabButton(AscentOptimizerViewState viewState, AscentOptimizerTab tab, string label)
    {
        if (GUILayout.Button(label))
        {
            viewState.SelectedTab = tab;
        }
    }

    private void DrawVehicleTab(AscentOptimizerViewState viewState)
    {
        if (GUILayout.Button("Refresh Vehicle Snapshot"))
        {
            viewState.CurrentVehicle = _controller.BuildVehicleProfile("Active Vessel");
            viewState.Diagnostics = _controller.CaptureDiagnostics();
            _windowState.StatusMessage = "Vehicle snapshot refreshed.";
        }

        if (viewState.CurrentVehicle is null)
        {
            return;
        }

        foreach (var stage in viewState.CurrentVehicle.Stages)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label(stage.StageName);
            GUILayout.Label($"Min throttle: {stage.MinThrottle01:0.00}");
            GUILayout.Label($"Max thrust: {stage.MaxThrustNewton:0.##} N");
            GUILayout.Label($"Isp vac/atm: {stage.IspVacSeconds:0.##} / {stage.IspAtmSeconds:0.##}");
            GUILayout.EndVertical();
        }
    }

    private void DrawGuidanceTab(AscentOptimizerViewState viewState)
    {
        if (viewState.CurrentVehicle is null)
        {
            GUILayout.Label("No vehicle profile is available.");
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Target orbit m:", GUILayout.Width(100));
        _windowState.TargetOrbitAltitudeText = GUILayout.TextField(_windowState.TargetOrbitAltitudeText);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Preview Orbit Guidance"))
        {
            ExecuteAndReport(() =>
            {
                viewState.CurrentGuidanceRequest = new GuidanceRequest
                {
                    Mode = GuidanceMode.AscentToOrbit,
                    TargetOrbitAltitudeMeters = ParseDouble(_windowState.TargetOrbitAltitudeText, 100000.0),
                };
                viewState.CurrentPolicy = _controller.PreviewGuidance(viewState.CurrentVehicle, viewState.CurrentGuidanceRequest);
            }, "Guidance preview updated.");
        }

        if (GUILayout.Button("Engage Orbit Guidance"))
        {
            ExecuteAndReport(() =>
            {
                viewState.CurrentGuidanceRequest ??= new GuidanceRequest
                {
                    Mode = GuidanceMode.AscentToOrbit,
                    TargetOrbitAltitudeMeters = ParseDouble(_windowState.TargetOrbitAltitudeText, 100000.0),
                };
                viewState.CurrentPolicy = _controller.EngageGuidance(viewState.CurrentVehicle, viewState.CurrentGuidanceRequest);
            }, "Guidance engaged.");
        }

        if (viewState.CurrentPolicy is not null)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label($"Max Q: {viewState.CurrentPolicy.RecommendedMaxDynamicPressurePa:0.##} Pa");
            GUILayout.Label($"Max Accel: {viewState.CurrentPolicy.RecommendedMaxAccelerationMetersPerSecondSquared:0.##} m/s^2");
            GUILayout.Label($"Autopilot: {viewState.CurrentPolicy.ShouldEngageAutopilot}");
            foreach (var note in viewState.CurrentPolicy.Notes)
            {
                GUILayout.Label(note);
            }
            GUILayout.EndVertical();
        }
    }

    private void DrawConstraintsTab(AscentOptimizerViewState viewState)
    {
        viewState.Diagnostics = _controller.CaptureDiagnostics();
        GUILayout.Label($"Current stage minimum throttle: {viewState.Diagnostics.CurrentStageMinimumThrottle01:0.00}");
        GUILayout.Label($"Estimated drag losses: {viewState.Diagnostics.EstimatedDragLossesMetersPerSecond:0.##} m/s");
        GUILayout.Label($"Surface hop guidance support: {viewState.Diagnostics.SupportsSurfaceHopGuidance}");
    }

    private void DrawDiagnosticsTab(AscentOptimizerViewState viewState)
    {
        viewState.Diagnostics = _controller.CaptureDiagnostics();
        GUILayout.Label($"Stage count: {viewState.Diagnostics.StageCount}");
        GUILayout.Label($"Min throttle: {viewState.Diagnostics.CurrentStageMinimumThrottle01:0.00}");
        GUILayout.Label($"Drag losses: {viewState.Diagnostics.EstimatedDragLossesMetersPerSecond:0.##} m/s");
        GUILayout.Label($"Surface hop support: {viewState.Diagnostics.SupportsSurfaceHopGuidance}");
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

    private static double ParseDouble(string text, double fallback)
    {
        return double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fallback;
    }
}
#endif