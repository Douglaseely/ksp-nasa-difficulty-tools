using System;
using System.Collections.Generic;

namespace KspAscentOptimizer.Integrations;

public sealed class GuidancePolicy
{
    public double RecommendedMaxDynamicPressurePa { get; set; }
    public double RecommendedMaxAccelerationMetersPerSecondSquared { get; set; }
    public bool ShouldEngageAutopilot { get; set; } = true;
    public IReadOnlyList<string> Notes { get; set; } = Array.Empty<string>();
}