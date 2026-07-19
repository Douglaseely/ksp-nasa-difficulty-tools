namespace KspMathCore.Ascent;

public sealed class AscentState
{
    public double TimeSeconds { get; set; }
    public double AltitudeMeters { get; set; }
    public double VelocityMetersPerSecond { get; set; }
    public double FlightPathAngleRadians { get; set; }
    public double MassKg { get; set; }
    public double DynamicPressurePa { get; set; }
    public double LocalGravityMetersPerSecondSquared { get; set; }
}