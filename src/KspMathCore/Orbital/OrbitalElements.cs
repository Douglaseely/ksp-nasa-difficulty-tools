namespace KspMathCore.Orbital;

public sealed class OrbitalElements
{
    public double SemiMajorAxisMeters { get; init; }
    public double Eccentricity { get; init; }
    public double InclinationRadians { get; init; }
    public double LongitudeOfAscendingNodeRadians { get; init; }
    public double ArgumentOfPeriapsisRadians { get; init; }
    public double TrueAnomalyRadians { get; init; }
}