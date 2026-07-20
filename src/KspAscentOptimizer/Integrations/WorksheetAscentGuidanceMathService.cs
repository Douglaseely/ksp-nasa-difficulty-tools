using System;
using KspAscentOptimizer.Optimization;

namespace KspAscentOptimizer.Integrations;

public sealed class WorksheetAscentGuidanceMathService : IAscentGuidanceMathService
{
    public GuidancePolicy BuildGuidancePolicy(VehicleProfile vehicle, GuidanceRequest? guidanceRequest, AscentOptimizationContext context)
    {
        if (vehicle is null)
        {
            throw new ArgumentNullException(nameof(vehicle));
        }

        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return guidanceRequest?.Mode == GuidanceMode.SurfaceHop
            ? BuildSurfaceHopPolicy(vehicle, guidanceRequest, context)
            : BuildAscentToOrbitPolicy(vehicle, guidanceRequest, context);
    }

    public GuidancePolicy BuildAscentToOrbitPolicy(VehicleProfile vehicle, GuidanceRequest? guidanceRequest, AscentOptimizationContext context)
    {
        _ = vehicle;
        _ = guidanceRequest;
        _ = context;
        throw new NotImplementedException("Implement ascent-to-orbit policy generation with AscentOptimizationWorksheet and adapter-derived vehicle constraints.");
    }

    public GuidancePolicy BuildSurfaceHopPolicy(VehicleProfile vehicle, GuidanceRequest? guidanceRequest, AscentOptimizationContext context)
    {
        _ = vehicle;
        _ = guidanceRequest;
        _ = context;
        throw new NotImplementedException("Implement same-body hop feasibility and policy generation using SurfaceHopWorksheet plus ascent constraints.");
    }
}