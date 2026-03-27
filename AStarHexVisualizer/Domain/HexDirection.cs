namespace AStarHexVisualizer.Domain;

/// <summary>
/// The 6 possible movement directions on a pointy-top hex grid.
/// Named from the perspective of a compass rose.
/// </summary>
public enum HexDirection
{
    NorthEast,
    East,
    SouthEast,
    SouthWest,
    West,
    NorthWest
}