namespace KspMathCore.Ascent;

public sealed class AscentState
{
    public double TimeSeconds { get; init; }
    public double AltitudeMeters { get; init; }
    public double VelocityMetersPerSecond { get; init; }
    public double FlightPathAngleRadians { get; init; }
    public double MassKg { get; init; }
    public double DynamicPressurePa { get; init; }
    public double LocalGravityMetersPerSecondSquared { get; init; }
}