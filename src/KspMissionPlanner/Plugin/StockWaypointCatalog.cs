#if KSP_RUNTIME
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using KspIntegration.Mission;
using KspIntegration.Models;
using KspIntegration.Reflection;

namespace KspMissionPlanner.Plugin;

public sealed class StockWaypointCatalog : IWaypointCatalog
{
    private readonly Type? _waypointManagerType;
    private readonly MethodInfo? _instanceMethod;
    private readonly PropertyInfo? _waypointsProperty;

    public StockWaypointCatalog()
    {
        _waypointManagerType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(assembly => assembly.GetType("FinePrint.WaypointManager", throwOnError: false))
            .FirstOrDefault(type => type is not null);

        _instanceMethod = _waypointManagerType?.GetMethod("Instance", BindingFlags.Public | BindingFlags.Static);
        _waypointsProperty = _waypointManagerType?.GetProperty("Waypoints", BindingFlags.Public | BindingFlags.Instance);
    }

    public bool IsAvailable => _waypointManagerType is not null && _instanceMethod is not null && _waypointsProperty is not null;

    public bool TryGetWaypoint(string bodyName, string waypointName, out WaypointSnapshot waypoint)
    {
        if (!IsAvailable)
        {
            waypoint = new WaypointSnapshot();
            return false;
        }

        var manager = _instanceMethod!.Invoke(null, null);
        var waypoints = _waypointsProperty!.GetValue(manager, null) as IEnumerable;
        if (waypoints is null)
        {
            waypoint = new WaypointSnapshot();
            return false;
        }

        foreach (var runtimeWaypoint in waypoints)
        {
            if (runtimeWaypoint is null)
            {
                continue;
            }

            var reader = new ReflectionObjectReader(runtimeWaypoint);
            var currentBody = reader.GetOptionalString(string.Empty, "celestialName", "BodyName", "bodyName");
            var currentName = reader.GetOptionalString(string.Empty, "name", "Name", "WaypointName", "waypointName");
            if (!string.Equals(currentBody, bodyName, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(currentName, waypointName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            waypoint = new WaypointSnapshot
            {
                BodyName = currentBody,
                Name = currentName,
                BiomeName = reader.GetOptionalString(string.Empty, "biome", "BiomeName", "biomeName"),
                LatitudeDegrees = reader.GetOptionalDouble(0.0, "latitude", "Latitude", "LatitudeDegrees"),
                LongitudeDegrees = reader.GetOptionalDouble(0.0, "longitude", "Longitude", "LongitudeDegrees"),
                AltitudeMeters = reader.GetOptionalDouble(0.0, "altitude", "Altitude", "AltitudeMeters"),
            };
            return true;
        }

        waypoint = new WaypointSnapshot();
        return false;
    }
}
#endif