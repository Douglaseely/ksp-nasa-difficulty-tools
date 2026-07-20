using KspAscentOptimizer.Integrations;
using KspAscentOptimizer.Optimization;

namespace KspAscentOptimizer.Plugin;

public sealed class AscentOptimizerViewState
{
    public AscentOptimizerTab SelectedTab { get; set; } = AscentOptimizerTab.Vehicle;
    public VehicleProfile? CurrentVehicle { get; set; }
    public GuidanceRequest? CurrentGuidanceRequest { get; set; }
    public GuidancePolicy? CurrentPolicy { get; set; }
    public AscentOptimizerDiagnosticsSnapshot Diagnostics { get; set; } = new AscentOptimizerDiagnosticsSnapshot();
}