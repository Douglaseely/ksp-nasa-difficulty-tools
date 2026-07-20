using KspAscentOptimizer.Optimization;

namespace KspAscentOptimizer.Integrations;

public interface IAscentGuidanceMathService
{
    GuidancePolicy BuildGuidancePolicy(VehicleProfile vehicle, GuidanceRequest? guidanceRequest, AscentOptimizationContext context);
}