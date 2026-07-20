using System;
using System.Collections.Generic;
using KspIntegration.Models;

namespace KspAscentOptimizer.Integrations;

public sealed class AscentOptimizationContext
{
    public IReadOnlyList<StageSnapshot> RuntimeStages { get; set; } = Array.Empty<StageSnapshot>();
    public AerodynamicSnapshot Aerodynamics { get; set; } = new AerodynamicSnapshot();
    public double CurrentStageMinimumThrottle01 { get; set; }
    public bool SupportsSurfaceHopGuidance { get; set; }
}