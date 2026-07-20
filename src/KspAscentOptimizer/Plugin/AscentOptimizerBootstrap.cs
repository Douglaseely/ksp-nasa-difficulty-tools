using KspIntegration.Flight;
using KspAscentOptimizer.Integrations;
using KspAscentOptimizer.Optimization;

namespace KspAscentOptimizer.Plugin;

public sealed class AscentOptimizerBootstrap
{
    private readonly IMechJebAdapter _mechJeb;
    private readonly IRealFuelsAdapter _realFuels;
    private readonly IFarAdapter _far;
    private readonly IAscentGuidanceMathService _guidanceMath;

    public AscentOptimizerBootstrap(
        IMechJebAdapter mechJeb,
        IRealFuelsAdapter realFuels,
        IFarAdapter far,
        IAscentGuidanceMathService? guidanceMath = null)
    {
        _mechJeb = mechJeb;
        _realFuels = realFuels;
        _far = far;
        _guidanceMath = guidanceMath ?? new WorksheetAscentGuidanceMathService();
    }

    public GuidancePolicy PreviewGuidance(VehicleProfile vehicle, GuidanceRequest? guidanceRequest = null)
    {
        var context = new AscentOptimizationContext
        {
            RuntimeStages = _realFuels.CaptureStages(),
            CurrentStageMinimumThrottle01 = _realFuels.GetCurrentStageMinimumThrottle01(),
            Aerodynamics = _far.CaptureAerodynamics(),
            SupportsSurfaceHopGuidance = guidanceRequest?.Mode == GuidanceMode.SurfaceHop && _mechJeb.SupportsSurfaceHopGuidance(),
        };

        return _guidanceMath.BuildGuidancePolicy(vehicle, guidanceRequest, context);
    }

    public GuidancePolicy ConfigureAndEngage(VehicleProfile vehicle, GuidanceRequest? guidanceRequest = null)
    {
        var policy = PreviewGuidance(vehicle, guidanceRequest);

        _mechJeb.SetAscentMaxAcceleration(policy.RecommendedMaxAccelerationMetersPerSecondSquared);
        _mechJeb.SetAscentLimitQ(policy.RecommendedMaxDynamicPressurePa);

        if (policy.ShouldEngageAutopilot)
        {
            _mechJeb.EngageAscentAutopilot();
        }

        return policy;
    }
}