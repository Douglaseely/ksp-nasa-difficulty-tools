namespace KspMissionPlanner.Integrations;

public interface IKspGameContext
{
    double UniversalTimeSeconds { get; }
    string CurrentSaveName { get; }
}

public interface IWaypointCatalog
{
    bool TryGetWaypoint(string bodyName, string waypointName, out WaypointRecord waypoint);
}

public sealed class WaypointRecord
{
    public string BodyName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double LatitudeDegrees { get; set; }
    public double LongitudeDegrees { get; set; }
    public double AltitudeMeters { get; set; }
}

public interface IManeuverNodeWriter
{
    void AddNode(double universalTimeSeconds, double prograde, double normal, double radial);
}