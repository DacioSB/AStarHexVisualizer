namespace AStarHexVisualizer.Domain;

/// <summary>
/// Represents a single hexagonal tile in the grid.
/// Stores both world data (type, position) and A* algorithm state (G, H, F, parent).
/// </summary>
public class HexTile
{
    public int Col { get; init; }
    public int Row { get; init; }

    public TileType Type { get; set; }

    /// <summary>
    /// G: Actual cost from the Start tile to this tile.
    /// </summary>
    public double G { get; set; } = double.MaxValue;

    /// <summary>
    /// H: Heuristic estimated cost from this tile to the Goal.
    /// </summary>
    public double H { get; set; } = 0;

    /// <summary> F: Total estimated cost of the cheapest solution through this tile (F = G + H). </summary>
    public double F => G + H;

    /// <summary>
    /// The tile we came from — used to reconstruct the final path.
    /// </summary>
    public HexTile? Parent { get; set; }

    public TileState State { get; set; } = TileState.Unvisited;

    /// <summary>
    /// True once this tile has been popped from the open list and fully evaluated.
    /// Maps to TileState.Closed but kept explicit for algorithm clarity.
    /// </summary>
    public bool IsVisited { get; set; } = false;

    /// <summary>
    /// True while this tile is in the open list (candidate for evaluation).
    /// </summary>
    public bool IsInOpenList { get; set; } = false;

    /// <summary>
    /// True after path reconstruction — this tile is on the optimal path.
    /// </summary>
    public bool IsOnFinalPath { get; set; } = false;

    public HexTile(int col, int row, TileType type = TileType.Empty)
    {
        Col = col;
        Row = row;
        Type = type;
    }

    /// <summary>
    /// Resets all A* state back to initial values.
    /// Call this when resetting the grid for a new run.
    /// </summary>
    public void ResetState()
    {
        G = double.MaxValue;
        H = 0;
        Parent = null;
        State = TileState.Unvisited;
        IsVisited = false;
        IsInOpenList = false;
        IsOnFinalPath = false;
    }

    /// <summary>
    /// A tile is walkable if it is not a Rock.
    /// </summary>
    public bool IsWalkable => Type != TileType.Rock;

    public override string ToString() => $"HexTile({Col},{Row}) Type={Type} F={F:F1} G={G:F1} H={H:F1}";

    /// <summary>
    /// Returns the movement cost to enter this tile.
    /// </summary>
    public double MovementCost => Type switch
    {
        TileType.Empty => MovementCosts.Empty,
        TileType.Slime => MovementCosts.Slime,
        TileType.Rock  => MovementCosts.Rock,
        TileType.Start => MovementCosts.Start,
        TileType.Goal  => MovementCosts.Goal,
        _ => throw new InvalidOperationException($"Unknown tile type: {Type}")
    };
}