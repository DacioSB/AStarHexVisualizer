namespace AStarHexVisualizer.Domain;

/// <summary>
/// Produces named preset HexGrids for demonstration purposes.
/// Each scenario is hand-crafted to highlight a specific
/// behavior of the A* algorithm.
///
/// All grids are 10x10. Start is always top-left (0,0).
/// Goal is always bottom-right (9,9).
/// </summary>
public static class GridFactory
{
    public const int GridSize = 10;

    // ── Public API ────────────────────────────────────────────

    /// <summary>
    /// Creates and returns a fully configured HexGrid
    /// for the given scenario. Each call returns a fresh instance.
    /// </summary>
    public static HexGrid Create(ScenarioType scenario) => scenario switch
    {
        ScenarioType.SimpleOpen    => CreateSimpleOpen(),
        ScenarioType.RockMaze      => CreateRockMaze(),
        ScenarioType.SlimePenalty  => CreateSlimePenalty(),
        _ => throw new ArgumentOutOfRangeException(
                nameof(scenario),
                scenario,
                $"No grid factory defined for scenario '{scenario}'.")
    };

    // ── Scenarios ─────────────────────────────────────────────

    /// <summary>
    /// SimpleOpen — flat empty field.
    /// A* expands cleanly in a diamond wave toward the goal.
    /// No obstacles to reason about — ideal for first-time viewing.
    /// </summary>
    private static HexGrid CreateSimpleOpen()
    {
        var grid = CreateBaseGrid();
        return grid;
    }

    /// <summary>
    /// Creates a blank 10x10 grid with Start at (0,0) and Goal at (9,9).
    /// All other tiles are Empty.
    /// </summary>
    private static HexGrid CreateBaseGrid()
    {
        var grid = new HexGrid(GridSize, GridSize);
        grid.SetTileType(0,            0,            TileType.Start);
        grid.SetTileType(GridSize - 1, GridSize - 1, TileType.Goal);
        return grid;
    }
    /// <summary>
    /// RockMaze — a vertical rock wall from row 2 to row 7,
    /// starting at col 3. A single gap at row 8 forces a detour
    /// down and around before heading to the goal.
    /// </summary>
    private static HexGrid CreateRockMaze()
    {
        var baseGrid = CreateBaseGrid();

        for (int row = 2; row <= 7; row++)
        {
            baseGrid.SetTileType(3, row, TileType.Rock);
        }
        for (int column = 4; column <= 8; column++)
        {
            baseGrid.SetTileType(column, 2, TileType.Rock);
        }
        return baseGrid;
    }
    /// <summary>
    /// SlimePenalty — slime tiles run diagonally from (1,1) to (8,8),
    /// two tiles wide. The direct path to the goal is expensive.
    /// A* finds the cheaper route skirting around the slime band.
    /// </summary>
    private static HexGrid CreateSlimePenalty()
    {
        var baseGrid = CreateBaseGrid();
        for (int i = 0; i <= 8; i++)
        {
            PlaceSlimeSafe(baseGrid, i, i);
            PlaceSlimeSafe(baseGrid, i + 1, i);
        }
        return baseGrid;
    }

    /// <summary>
    /// Places a Slime tile only if the position is within bounds
    /// and not already occupied by Start or Goal.
    /// </summary>
    private static void PlaceSlimeSafe(HexGrid grid, int col, int row)
    {
        HexTile? hexTile = grid.GetTile(col, row);
        if (hexTile is null) return;
        if (hexTile.Type is TileType.Start or TileType.Goal) return;
        grid.SetTileType(col, row, TileType.Slime); 
    }
}