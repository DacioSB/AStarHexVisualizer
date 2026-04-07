namespace AStarHexVisualizer.Domain;

/// <summary>
/// Maps a TileType to its movement cost for the A* algorithm.
/// This is the single place where terrain cost rules live.
///
/// Called during g(n) calculation: cost to enter a neighbor tile.
/// </summary>
public static class MovementCostProvider
{
    public static double GetCost(TileType tileType)
    {
        return tileType switch
        {
            TileType.Empty => MovementCosts.Empty,
            TileType.Slime => MovementCosts.Slime,
            TileType.Rock  => MovementCosts.Rock,
            TileType.Start => MovementCosts.Start,
            TileType.Goal  => MovementCosts.Goal,
            _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, $"Unsupported tile type: {tileType}")
        };
    }

    public static bool IsPassable(TileType tileType) => GetCost(tileType) < MovementCosts.Rock;
}