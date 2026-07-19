namespace KspMathCore.Orbital;

public sealed class OrbitalElements
{
    public double SemiMajorAxisMeters { get; set; }
    public double Eccentricity { get; set; }
    public double InclinationRadians { get; set; }
    public double LongitudeOfAscendingNodeRadians { get; set; }
    public double ArgumentOfPeriapsisRadians { get; set; }
    public double TrueAnomalyRadians { get; set; }
}