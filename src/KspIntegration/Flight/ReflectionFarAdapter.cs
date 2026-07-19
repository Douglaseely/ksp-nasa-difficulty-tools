using System;
using KspIntegration.Models;
using KspIntegration.Reflection;

namespace KspIntegration.Flight;

public sealed class ReflectionFarAdapter : IFarAdapter
{
    private readonly object _runtimeFlightData;

    public ReflectionFarAdapter(object runtimeFlightData)
    {
        _runtimeFlightData = runtimeFlightData ?? throw new ArgumentNullException(nameof(runtimeFlightData));
    }

    public AerodynamicSnapshot CaptureAerodynamics()
    {
        var reader = new ReflectionObjectReader(_runtimeFlightData);
        return new AerodynamicSnapshot
        {
            DynamicPressurePa = reader.GetOptionalDouble(0.0, "DynamicPressurePa", "dynamicPressurePa", "DynamicPressure", "dynamicPressure"),
            AtmosphericDensityKgPerCubicMeter = reader.GetOptionalDouble(0.0, "AtmosphericDensityKgPerCubicMeter", "atmosphericDensityKgPerCubicMeter", "Density", "density"),
            MachNumber = reader.GetOptionalDouble(0.0, "MachNumber", "machNumber", "Mach", "mach"),
            EstimatedDragLossesMetersPerSecond = reader.GetOptionalDouble(0.0, "EstimatedDragLossesMetersPerSecond", "estimatedDragLossesMetersPerSecond", "DragLossEstimate", "dragLossEstimate"),
        };
    }

    public double EstimateDragLossesMetersPerSecond()
    {
        return CaptureAerodynamics().EstimatedDragLossesMetersPerSecond;
    }
}