#if KSP_RUNTIME
using System;
using System.Collections.Generic;
using System.Linq;

namespace KspAscentOptimizer.Plugin;

public sealed class StockStageProviderRuntime
{
    public double CurrentStageMinimumThrottle01 { get; set; }
    public IReadOnlyList<StockStageRuntimeData> Stages { get; set; } = Array.Empty<StockStageRuntimeData>();

    public static StockStageProviderRuntime CaptureActiveVessel()
    {
        var vessel = FlightGlobals.ActiveVessel;
        if (vessel is null)
        {
            return new StockStageProviderRuntime();
        }

        var stageMap = new Dictionary<int, List<ModuleEngines>>();
        foreach (var part in vessel.parts)
        {
            foreach (var engine in part.FindModulesImplementing<ModuleEngines>())
            {
                if (!stageMap.TryGetValue(part.inverseStage, out var engines))
                {
                    engines = new List<ModuleEngines>();
                    stageMap[part.inverseStage] = engines;
                }

                engines.Add(engine);
            }
        }

        var stages = new List<StockStageRuntimeData>();
        foreach (var kvp in stageMap.OrderByDescending(entry => entry.Key))
        {
            var engines = new List<StockEngineRuntimeData>();
            double maxThrustNewton = 0.0;
            double minThrottle01 = 0.0;
            double ispVacSeconds = 0.0;
            double ispAtmSeconds = 0.0;
            foreach (var engine in kvp.Value)
            {
                var engineData = new StockEngineRuntimeData
                {
                    EngineName = engine.engineID,
                    MaxThrustNewton = engine.maxThrust * 1000.0,
                    MinThrottle01 = engine.throttleLocked ? 1.0 : 0.0,
                    IspVacSeconds = engine.atmosphereCurve.Evaluate(0f),
                    IspAtmSeconds = engine.atmosphereCurve.Evaluate(1f),
                    RemainingIgnitions = 0,
                    RequiresUllage = false,
                    PressureFed = false,
                };

                engines.Add(engineData);
                maxThrustNewton += engineData.MaxThrustNewton;
                minThrottle01 = engines.Count == 1 ? engineData.MinThrottle01 : Math.Min(minThrottle01, engineData.MinThrottle01);
                ispVacSeconds = Math.Max(ispVacSeconds, engineData.IspVacSeconds);
                ispAtmSeconds = Math.Max(ispAtmSeconds, engineData.IspAtmSeconds);
            }

            var stageParts = vessel.parts.Where(part => part.inverseStage == kvp.Key).ToList();
            stages.Add(new StockStageRuntimeData
            {
                StageIndex = kvp.Key,
                StageName = $"Stage {kvp.Key}",
                StartMassKg = stageParts.Sum(GetWetMassKg),
                EndMassKg = stageParts.Sum(part => part.mass * 1000.0),
                MaxThrustNewton = maxThrustNewton,
                MinThrottle01 = minThrottle01,
                IspVacSeconds = ispVacSeconds,
                IspAtmSeconds = ispAtmSeconds,
                RemainingIgnitions = 0,
                Engines = engines,
            });
        }

        var currentStage = stages.FirstOrDefault(stage => stage.StageIndex == vessel.currentStage);
        return new StockStageProviderRuntime
        {
            CurrentStageMinimumThrottle01 = currentStage?.MinThrottle01 ?? 0.0,
            Stages = stages,
        };
    }

    private static double GetWetMassKg(Part part)
    {
        return (part.mass + part.GetResourceMass()) * 1000.0;
    }
}
#endif