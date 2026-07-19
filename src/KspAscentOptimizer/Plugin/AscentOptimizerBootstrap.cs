using KspAscentOptimizer.Integrations;
using KspAscentOptimizer.Optimization;

namespace KspAscentOptimizer.Plugin;

public sealed class AscentOptimizerBootstrap
{
    private readonly IMechJebAdapter _mechJeb;
    private readonly IRealFuelsAdapter _realFuels;
    private readonly IFarAdapter _far;

    public AscentOptimizerBootstrap(IMechJebAdapter mechJeb, IRealFuelsAdapter realFuels, IFarAdapter far)
    {
        _mechJeb = mechJeb;
        _realFuels = realFuels;
        _far = far;
    }

    public void ConfigureAndEngage(VehicleProfile vehicle)
    {
        _ = vehicle;
        _ = _realFuels.GetCurrentStageMinimumThrottle01();
        _ = _far.EstimateDragLossesMetersPerSecond();

        // Placeholder policy until your math optimizer is implemented.
        _mechJeb.SetAscentMaxAcceleration(30.0);
        _mechJeb.SetAscentLimitQ(25000.0);
        _mechJeb.EngageAscentAutopilot();
    }
}