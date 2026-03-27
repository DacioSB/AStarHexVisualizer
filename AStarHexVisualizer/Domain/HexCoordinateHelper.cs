namespace AStarHexVisualizer.Domain;

/// <summary>
/// Provides hex grid coordinate math for a pointy-top odd-r offset grid.
///
/// In odd-r offset:
///   - Hexagons are pointy-top oriented
///   - ODD rows are shifted to the right by half a tile
///   - Neighbor offsets differ depending on whether the current row is odd or even
///
///  Even row:             Odd row:
///  NW(-1,-1) NE(0,-1)   NW(0,-1) NE(+1,-1)
///   W(-1, 0)  E(+1, 0)   W(-1, 0)  E(+1, 0)
///  SW(-1,+1) SE(0,+1)   SW(0,+1) SE(+1,+1)
/// </summary>
public static class HexCoordinateHelper
{

    private static readonly (int dCol, int dRow)[] EvenRowOffsets =
    [
        ( 0, -1),  // NorthEast
        ( 1,  0),  // East
        ( 0,  1),  // SouthEast
        (-1,  1),  // SouthWest
        (-1,  0),  // West
        (-1, -1),  // NorthWest
    ];

    private static readonly (int dCol, int dRow)[] OddRowOffsets =
    [
        ( 1, -1),  // NorthEast
        ( 1,  0),  // East
        ( 1,  1),  // SouthEast
        ( 0,  1),  // SouthWest
        (-1,  0),  // West
        ( 0, -1),  // NorthWest
    ];

    // ── Public API ────────────────────────────────────────────

    /// <summary>
    /// Returns all valid neighbor tiles of the given tile within the grid bounds.
    /// Skips neighbors that fall outside the grid — no null entries in the result.
    /// </summary>
    public static IEnumerable<HexTile> GetNeighbors(HexTile tile, HexGrid grid)
    {
        var offsets = IsOddRow(tile.Row) ? OddRowOffsets : EvenRowOffsets;

        foreach (var (dCol, dRow) in offsets)
        {
            var neighbor = grid.GetTile(tile.Col + dCol, tile.Row + dRow);
            if (neighbor is not null)
                yield return neighbor;
        }
    }

    /// <summary>
    /// Returns the neighbor of a tile in a specific direction.
    /// Returns null if that neighbor is outside the grid bounds.
    /// </summary>
    public static HexTile? GetNeighborInDirection(
        HexTile tile,
        HexDirection direction,
        HexGrid grid)
    {
        var offsets = IsOddRow(tile.Row) ? OddRowOffsets : EvenRowOffsets;
        var (dCol, dRow) = offsets[(int)direction];

        return grid.GetTile(tile.Col + dCol, tile.Row + dRow);
    }

    /// <summary>
    /// Returns the number of valid neighbors (useful for edge/corner detection).
    /// </summary>
    public static int CountNeighbors(HexTile tile, HexGrid grid)
        => GetNeighbors(tile, grid).Count();

    // ── Private helpers ───────────────────────────────────────

    private static bool IsOddRow(int row) => (row % 2) != 0;
}