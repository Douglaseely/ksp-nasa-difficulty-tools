namespace KspMissionPlanner.Integrations;

public interface IKspGameContext
{
    double UniversalTimeSeconds { get; }
    string CurrentSaveName { get; }
}

public interface IManeuverNodeWriter
{
    void AddNode(double universalTimeSeconds, double prograde, double normal, double radial);
}