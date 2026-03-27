namespace AStarHexVisualizer.Domain;

/// <summary>
/// Holds the full 2D collection of HexTiles and exposes
/// grid-level operations like tile access and full reset.
/// </summary>
public class HexGrid
{
    public int Cols { get; }
    public int Rows { get; }

    private readonly HexTile[,] _tiles;

    public HexTile? StartTile { get; private set; }
    public HexTile? GoalTile { get; private set; }

    public HexGrid(int cols, int rows)
    {
        Cols = cols;
        Rows = rows;
        _tiles = new HexTile[Cols, Rows];

        for (int col = 0; col < Cols; col++)
        {
            for (int row = 0; row < Rows; row++)
            {
                _tiles[col, row] = new HexTile(col, row);
            }
        }
    }

    /// <summary>
    /// Returns the tile at (col, row). Returns null if out of bounds.
    /// </summary>
    public HexTile? GetTile(int col, int row)
    {
        if (col < 0 || col >= Cols || row < 0 || row >= Rows)
            return null;
        return _tiles[col, row];
    }

    /// <summary>
    /// Sets the type of a tile and tracks Start/Goal references.
    /// </summary>
    public void SetTileType(int col, int row, TileType type)
    {
        var tile = GetTile(col, row) ?? throw new ArgumentOutOfRangeException($"Tile coordinates ({col}, {row}) are out of bounds.");

        // Clear previous Start/Goal references if we're overwriting them
        if (tile.Type == TileType.Start) StartTile = null;
        if (tile.Type == TileType.Goal)  GoalTile  = null;

        tile.Type = type;

        // Update Start/Goal references if applicable
        if (type == TileType.Start) StartTile = tile;
        if (type == TileType.Goal)  GoalTile  = tile;
    }

    /// <summary>
    /// Enumerates every tile in the grid (row-major order).
    /// </summary>
    public IEnumerable<HexTile> GetAllTiles()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                yield return _tiles[col, row];
            }
        }
    }

    /// <summary>
    /// Resets all A* algorithm state on every tile.
    /// Terrain (TileType) is preserved — only algorithm state is cleared.
    /// </summary>
    public void ResetAlgorithmState()
    {
        foreach (var tile in GetAllTiles())
            tile.ResetState();
    }

    public override string ToString() => $"HexGrid({Cols}x{Rows})";
}