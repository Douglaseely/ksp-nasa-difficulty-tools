#if KSP_RUNTIME
namespace KspAscentOptimizer.Plugin;

public sealed class StockEngineRuntimeData
{
    public string EngineName { get; set; } = string.Empty;
    public double MaxThrustNewton { get; set; }
    public double MinThrottle01 { get; set; }
    public double IspVacSeconds { get; set; }
    public double IspAtmSeconds { get; set; }
    public int RemainingIgnitions { get; set; }
    public bool RequiresUllage { get; set; }
    public bool PressureFed { get; set; }
}
#endif