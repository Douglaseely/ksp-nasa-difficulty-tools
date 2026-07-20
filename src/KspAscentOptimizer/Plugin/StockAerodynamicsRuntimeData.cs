#if KSP_RUNTIME
namespace KspAscentOptimizer.Plugin;

public sealed class StockAerodynamicsRuntimeData
{
    public double DynamicPressure { get; set; }
    public double Density { get; set; }
    public double Mach { get; set; }
    public double DragLossEstimate { get; set; }

    public static StockAerodynamicsRuntimeData CaptureActiveVessel()
    {
        var vessel = FlightGlobals.ActiveVessel;
        if (vessel is null)
        {
            return new StockAerodynamicsRuntimeData();
        }

        var density = vessel.atmDensity;
        var speed = vessel.srfSpeed;
        return new StockAerodynamicsRuntimeData
        {
            DynamicPressure = 0.5 * density * speed * speed,
            Density = density,
            Mach = vessel.mach,
            DragLossEstimate = 0.0,
        };
    }
}
#endif