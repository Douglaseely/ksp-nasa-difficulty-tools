using System;
using System.Collections.Generic;
using KspAscentOptimizer.Integrations;
using KspAscentOptimizer.Optimization;
using KspIntegration.Flight;

namespace KspAscentOptimizer.Plugin;

public sealed class AscentOptimizerController
{
    private readonly AscentOptimizerBootstrap _bootstrap;
    private readonly IRealFuelsAdapter _realFuels;
    private readonly IFarAdapter _far;
    private readonly IMechJebAdapter _mechJeb;

    public AscentOptimizerController(
        AscentOptimizerBootstrap bootstrap,
        IRealFuelsAdapter realFuels,
        IFarAdapter far,
        IMechJebAdapter mechJeb)
    {
        _bootstrap = bootstrap ?? throw new ArgumentNullException(nameof(bootstrap));
        _realFuels = realFuels ?? throw new ArgumentNullException(nameof(realFuels));
        _far = far ?? throw new ArgumentNullException(nameof(far));
        _mechJeb = mechJeb ?? throw new ArgumentNullException(nameof(mechJeb));
    }

    public AscentOptimizerViewState CreateInitialViewState()
    {
        return new AscentOptimizerViewState
        {
            CurrentVehicle = BuildVehicleProfile("Active Vessel"),
            Diagnostics = CaptureDiagnostics(),
        };
    }

    public VehicleProfile BuildVehicleProfile(string craftName)
    {
        var stages = new List<StagePerformance>();
        foreach (var stage in _realFuels.CaptureStages())
        {
            stages.Add(new StagePerformance
            {
                StageName = stage.StageName,
                MinThrottle01 = stage.MinThrottle01,
                MaxThrustNewton = stage.MaxThrustNewton,
                IspVacSeconds = stage.IspVacSeconds,
                IspAtmSeconds = stage.IspAtmSeconds,
            });
        }

        return new VehicleProfile
        {
            CraftName = string.IsNullOrWhiteSpace(craftName) ? "Active Vessel" : craftName.Trim(),
            Stages = stages,
        };
    }

    public GuidancePolicy PreviewGuidance(VehicleProfile vehicle, GuidanceRequest? guidanceRequest = null)
    {
        return _bootstrap.PreviewGuidance(vehicle, guidanceRequest);
    }

    public GuidancePolicy EngageGuidance(VehicleProfile vehicle, GuidanceRequest? guidanceRequest = null)
    {
        return _bootstrap.ConfigureAndEngage(vehicle, guidanceRequest);
    }

    public AscentOptimizerDiagnosticsSnapshot CaptureDiagnostics()
    {
        return new AscentOptimizerDiagnosticsSnapshot
        {
            StageCount = _realFuels.CaptureStages().Count,
            CurrentStageMinimumThrottle01 = _realFuels.GetCurrentStageMinimumThrottle01(),
            EstimatedDragLossesMetersPerSecond = _far.EstimateDragLossesMetersPerSecond(),
            SupportsSurfaceHopGuidance = _mechJeb.SupportsSurfaceHopGuidance(),
        };
    }
}