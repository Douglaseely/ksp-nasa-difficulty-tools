#if KSP_RUNTIME
using System;
using System.Collections.Generic;

namespace KspAscentOptimizer.Plugin;

public sealed class StockStageRuntimeData
{
    public int StageIndex { get; set; }
    public string StageName { get; set; } = string.Empty;
    public double StartMassKg { get; set; }
    public double EndMassKg { get; set; }
    public double MaxThrustNewton { get; set; }
    public double MinThrottle01 { get; set; }
    public double IspVacSeconds { get; set; }
    public double IspAtmSeconds { get; set; }
    public int RemainingIgnitions { get; set; }
    public IReadOnlyList<StockEngineRuntimeData> Engines { get; set; } = Array.Empty<StockEngineRuntimeData>();
}
#endif