using System;
using System.Collections.Generic;
using KspIntegration.Models;
using KspIntegration.Reflection;

namespace KspIntegration.Mission;

public sealed class ReflectionWaypointCatalog : IWaypointCatalog
{
    private readonly IReadOnlyList<object> _runtimeWaypoints;

    public ReflectionWaypointCatalog(IEnumerable<object> runtimeWaypoints)
    {
        if (runtimeWaypoints is null)
        {
            throw new ArgumentNullException(nameof(runtimeWaypoints));
        }

        var items = new List<object>();
        foreach (var waypoint in runtimeWaypoints)
        {
            if (waypoint is not null)
            {
                items.Add(waypoint);
            }
        }

        _runtimeWaypoints = items;
    }

    public bool TryGetWaypoint(string bodyName, string waypointName, out WaypointSnapshot waypoint)
    {
        foreach (var runtimeWaypoint in _runtimeWaypoints)
        {
            var reader = new ReflectionObjectReader(runtimeWaypoint);
            var currentBodyName = reader.GetOptionalString(string.Empty, "BodyName", "bodyName", "CelestialBody", "celestialBody");
            var currentWaypointName = reader.GetOptionalString(string.Empty, "Name", "name", "WaypointName", "waypointName");

            if (string.Equals(currentBodyName, bodyName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentWaypointName, waypointName, StringComparison.OrdinalIgnoreCase))
            {
                waypoint = new WaypointSnapshot
                {
                    BodyName = currentBodyName,
                    Name = currentWaypointName,
                    BiomeName = reader.GetOptionalString(string.Empty, "BiomeName", "biomeName", "Biome", "biome"),
                    LatitudeDegrees = reader.GetOptionalDouble(0.0, "LatitudeDegrees", "latitude", "Latitude"),
                    LongitudeDegrees = reader.GetOptionalDouble(0.0, "LongitudeDegrees", "longitude", "Longitude"),
                    AltitudeMeters = reader.GetOptionalDouble(0.0, "AltitudeMeters", "altitude", "Altitude"),
                };
                return true;
            }
        }

        waypoint = new WaypointSnapshot();
        return false;
    }
}