namespace AStarHexVisualizer.Domain;

/// <summary>
/// Defines what a tile IS in the world — its terrain type.
/// This affects movement cost and walkability.
/// </summary>
public enum TileType
{
    Empty,
    Rock,
    Slime,
    Start,
    Goal
}