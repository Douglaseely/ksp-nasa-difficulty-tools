namespace KspAscentOptimizer.Plugin;

public sealed class AscentOptimizerDiagnosticsSnapshot
{
    public int StageCount { get; set; }
    public double CurrentStageMinimumThrottle01 { get; set; }
    public double EstimatedDragLossesMetersPerSecond { get; set; }
    public bool SupportsSurfaceHopGuidance { get; set; }
}