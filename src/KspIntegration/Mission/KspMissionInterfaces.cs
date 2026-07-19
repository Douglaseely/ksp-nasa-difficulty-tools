using KspIntegration.Models;

namespace KspIntegration.Mission;

public interface IKspGameContext
{
    double UniversalTimeSeconds { get; }
    string CurrentSaveName { get; }
    GameStateSnapshot CaptureGameState();
}

public interface IWaypointCatalog
{
    bool TryGetWaypoint(string bodyName, string waypointName, out WaypointSnapshot waypoint);
}

public interface IManeuverNodeWriter
{
    void AddNode(ManeuverNodeRequest nodeRequest);
}