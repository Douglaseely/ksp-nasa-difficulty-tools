using System;
using System.Collections.Generic;
using KspIntegration.Models;
using KspIntegration.Reflection;

namespace KspIntegration.Flight;

public sealed class ReflectionRealFuelsAdapter : IRealFuelsAdapter
{
    private readonly object _runtimeStageProvider;

    public ReflectionRealFuelsAdapter(object runtimeStageProvider)
    {
        _runtimeStageProvider = runtimeStageProvider ?? throw new ArgumentNullException(nameof(runtimeStageProvider));
    }

    public IReadOnlyList<StageSnapshot> CaptureStages()
    {
        var reader = new ReflectionObjectReader(_runtimeStageProvider);
        var stages = new List<StageSnapshot>();
        foreach (var runtimeStage in reader.GetOptionalObjectList("Stages", "stages", "StageData", "stageData"))
        {
            stages.Add(MapStage(runtimeStage));
        }

        return stages;
    }

    public double GetCurrentStageMinimumThrottle01()
    {
        var reader = new ReflectionObjectReader(_runtimeStageProvider);
        return reader.GetOptionalDouble(0.0, "CurrentStageMinimumThrottle01", "currentStageMinimumThrottle01", "MinThrottle01", "minThrottle01");
    }

    private static StageSnapshot MapStage(object runtimeStage)
    {
        var reader = new ReflectionObjectReader(runtimeStage);
        var engines = new List<EngineSnapshot>();
        foreach (var runtimeEngine in reader.GetOptionalObjectList("Engines", "engines"))
        {
            var engine = new ReflectionObjectReader(runtimeEngine);
            engines.Add(new EngineSnapshot
            {
                EngineName = engine.GetOptionalString(string.Empty, "EngineName", "engineName", "Name", "name"),
                MaxThrustNewton = engine.GetOptionalDouble(0.0, "MaxThrustNewton", "maxThrustNewton", "MaxThrust", "maxThrust"),
                MinThrottle01 = engine.GetOptionalDouble(0.0, "MinThrottle01", "minThrottle", "MinThrottle"),
                IspVacSeconds = engine.GetOptionalDouble(0.0, "IspVacSeconds", "VacIsp", "vacIsp"),
                IspAtmSeconds = engine.GetOptionalDouble(0.0, "IspAtmSeconds", "AtmIsp", "atmIsp"),
                RemainingIgnitions = engine.GetOptionalInt32(0, "RemainingIgnitions", "IgnitionsRemaining", "ignitionsRemaining"),
                RequiresUllage = engine.GetOptionalBoolean(false, "RequiresUllage", "requiresUllage"),
                PressureFed = engine.GetOptionalBoolean(false, "PressureFed", "pressureFed"),
            });
        }

        return new StageSnapshot
        {
            StageIndex = reader.GetOptionalInt32(0, "StageIndex", "stageIndex", "Index", "index"),
            StageName = reader.GetOptionalString(string.Empty, "StageName", "stageName", "Name", "name"),
            StartMassKg = reader.GetOptionalDouble(0.0, "StartMassKg", "startMassKg", "StartMass", "startMass"),
            EndMassKg = reader.GetOptionalDouble(0.0, "EndMassKg", "endMassKg", "EndMass", "endMass"),
            MaxThrustNewton = reader.GetOptionalDouble(0.0, "MaxThrustNewton", "maxThrustNewton", "MaxThrust", "maxThrust"),
            MinThrottle01 = reader.GetOptionalDouble(0.0, "MinThrottle01", "minThrottle", "MinThrottle"),
            IspVacSeconds = reader.GetOptionalDouble(0.0, "IspVacSeconds", "VacIsp", "vacIsp"),
            IspAtmSeconds = reader.GetOptionalDouble(0.0, "IspAtmSeconds", "AtmIsp", "atmIsp"),
            RemainingIgnitions = reader.GetOptionalInt32(0, "RemainingIgnitions", "IgnitionsRemaining", "ignitionsRemaining"),
            Engines = engines,
        };
    }
}