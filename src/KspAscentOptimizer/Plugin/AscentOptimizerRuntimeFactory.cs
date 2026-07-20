#if KSP_RUNTIME
using KspAscentOptimizer.Integrations;
using KspIntegration.Flight;

namespace KspAscentOptimizer.Plugin;

public static class AscentOptimizerRuntimeFactory
{
    public static AscentOptimizerController CreateController()
    {
        var mechJeb = MechJebRuntimeLocator.TryCreateAdapter() ?? new NoOpMechJebAdapter();
        var realFuels = new ReflectionRealFuelsAdapter(StockStageProviderRuntime.CaptureActiveVessel());
        var far = new ReflectionFarAdapter(StockAerodynamicsRuntimeData.CaptureActiveVessel());
        var bootstrap = new AscentOptimizerBootstrap(mechJeb, realFuels, far, new WorksheetAscentGuidanceMathService());
        return new AscentOptimizerController(bootstrap, realFuels, far, mechJeb);
    }
}
#endif