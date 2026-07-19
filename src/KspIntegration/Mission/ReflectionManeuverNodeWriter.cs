using System;
using System.Reflection;
using KspIntegration.Models;

namespace KspIntegration.Mission;

public sealed class ReflectionManeuverNodeWriter : IManeuverNodeWriter
{
    private readonly object _runtimeNodeWriter;

    public ReflectionManeuverNodeWriter(object runtimeNodeWriter)
    {
        _runtimeNodeWriter = runtimeNodeWriter ?? throw new ArgumentNullException(nameof(runtimeNodeWriter));
    }

    public void AddNode(ManeuverNodeRequest nodeRequest)
    {
        if (nodeRequest is null)
        {
            throw new ArgumentNullException(nameof(nodeRequest));
        }

        var method = _runtimeNodeWriter.GetType().GetMethod(
            "AddNode",
            BindingFlags.Instance | BindingFlags.Public,
            null,
            new[]
            {
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
            },
            null);

        if (method is null)
        {
            throw new MissingMethodException(_runtimeNodeWriter.GetType().FullName, "AddNode");
        }

        method.Invoke(_runtimeNodeWriter, new object[]
        {
            nodeRequest.UniversalTimeSeconds,
            nodeRequest.ProgradeMetersPerSecond,
            nodeRequest.NormalMetersPerSecond,
            nodeRequest.RadialMetersPerSecond,
        });
    }
}