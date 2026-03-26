namespace AStarHexVisualizer.Domain;

/// <summary>
/// Defines all movement cost constants used by the A* algorithm.
/// </summary>

public static class MovementCosts
{
    public const double Empty    = 1.0;
    public const double Slime    = 3.0;
    public const double Rock     = double.MaxValue;
    public const double Start    = 1.0;
    public const double Goal     = 1.0;
}
