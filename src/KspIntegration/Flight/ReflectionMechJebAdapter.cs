using System;
using System.Linq;
using KspIntegration.Models;
using KspIntegration.Reflection;

namespace KspIntegration.Flight;

public sealed class ReflectionMechJebAdapter : IMechJebAdapter
{
    private readonly object _runtimeController;

    public ReflectionMechJebAdapter(object runtimeController)
    {
        _runtimeController = runtimeController ?? throw new ArgumentNullException(nameof(runtimeController));
    }

    public MechJebAscentSettingsSnapshot CaptureAscentSettings()
    {
        var reader = new ReflectionObjectReader(_runtimeController);
        return new MechJebAscentSettingsSnapshot
        {
            LimitQPa = reader.GetOptionalDouble(0.0, "LimitQPa", "limitQPa", "LimitQ", "limitQ"),
            MaxAccelerationMetersPerSecondSquared = reader.GetOptionalDouble(0.0, "MaxAccelerationMetersPerSecondSquared", "maxAccelerationMetersPerSecondSquared", "MaxAcceleration", "maxAcceleration"),
            AscentAutopilotEnabled = reader.GetOptionalBoolean(false, "AscentAutopilotEnabled", "ascentAutopilotEnabled", "AutopilotEnabled", "autopilotEnabled"),
            SupportsSurfaceHopGuidance = SupportsSurfaceHopGuidance(),
        };
    }

    public void SetAscentLimitQ(double maxDynamicPressure)
    {
        SetMember("LimitQPa", maxDynamicPressure, "LimitQ", "limitQ");
    }

    public void SetAscentMaxAcceleration(double maxAccelerationMetersPerSecondSquared)
    {
        SetMember("MaxAccelerationMetersPerSecondSquared", maxAccelerationMetersPerSecondSquared, "MaxAcceleration", "maxAcceleration");
    }

    public void EngageAscentAutopilot()
    {
        var method = _runtimeController.GetType().GetMethod("EngageAscentAutopilot");
        if (method is not null)
        {
            method.Invoke(_runtimeController, Array.Empty<object>());
            return;
        }

        SetMember("AscentAutopilotEnabled", true, "AutopilotEnabled", "autopilotEnabled");
    }

    public bool SupportsSurfaceHopGuidance()
    {
        var reader = new ReflectionObjectReader(_runtimeController);
        return reader.GetOptionalBoolean(false, "SupportsSurfaceHopGuidance", "supportsSurfaceHopGuidance");
    }

    private void SetMember(string preferredName, object value, params string[] alternateNames)
    {
        foreach (var memberName in new[] { preferredName }.Concat(alternateNames))
        {
            var property = _runtimeController.GetType().GetProperty(memberName);
            if (property is not null && property.CanWrite)
            {
                property.SetValue(_runtimeController, value, null);
                return;
            }

            var field = _runtimeController.GetType().GetField(memberName);
            if (field is not null)
            {
                field.SetValue(_runtimeController, value);
                return;
            }
        }

        throw new MissingMemberException(_runtimeController.GetType().FullName, preferredName);
    }
}