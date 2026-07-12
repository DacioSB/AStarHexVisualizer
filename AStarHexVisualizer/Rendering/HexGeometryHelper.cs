namespace AStarHexVisualizer.Rendering;

/// <summary>
/// Pure math helper for pointy-top hexagon geometry on an odd-r offset grid.
///
/// Coordinate conventions:
///   tileSize  = circumradius R (center to vertex, in pixels)
///   hexWidth  = sqrt(3) * R   (horizontal spacing between centers)
///   hexHeight = 2 * R         (tip-to-tip vertical height)
///   vertStep  = hexHeight * 3/4 (vertical distance between row centers)
///
/// Odd rows are shifted right by hexWidth/2 to produce the hex tiling.
///
/// No MAUI or UI dependencies — uses HexPoint and HexSize (own structs).
/// </summary>
public static class HexGeometryHelper
{
    // ── Constants ─────────────────────────────────────────────

    /// <summary>Default tile size (circumradius in pixels).</summary>
    public const float DefaultTileSize = 32f;

    private const double Deg30InRadians = Math.PI / 6.0;
    private const double Deg60InRadians = Math.PI / 3.0;

    // ── Public API ────────────────────────────────────────────

    /// <summary>
    /// Returns the screen-space center point of the hex tile at (col, row).
    /// </summary>
    public static HexPoint GetHexCenter(
        int col, int row,
        float tileSize,
        float offsetX = 0f,
        float offsetY = 0f)
    {
        var hexWidth  = HexWidth(tileSize);
        var hexHeight = HexHeight(tileSize);
        var vertStep  = VerticalStep(tileSize);

        var cx = col * hexWidth
                 + (IsOddRow(row) ? hexWidth / 2f : 0f)
                 + offsetX
                 + tileSize;

        var cy = row * vertStep
                 + offsetY
                 + hexHeight / 2f;

        return new HexPoint(cx, cy);
    }

    /// <summary>
    /// Returns the 6 vertex points of a pointy-top hexagon
    /// centered at the screen position of tile (col, row).
    ///
    /// Vertices ordered clockwise from top-right:
    ///   0=NE, 1=E, 2=SE, 3=SW, 4=W, 5=NW
    /// </summary>
    public static HexPoint[] GetHexPoints(
        int col, int row,
        float tileSize,
        float offsetX = 0f,
        float offsetY = 0f)
    {
        var center = GetHexCenter(col, row, tileSize, offsetX, offsetY);
        return GetHexPointsFromCenter(center, tileSize);
    }

    /// <summary>
    /// Returns the 6 vertex points of a pointy-top hexagon
    /// given an explicit center point.
    /// </summary>
    public static HexPoint[] GetHexPointsFromCenter(HexPoint center, float tileSize)
    {
        var points = new HexPoint[6];

        for (var i = 0; i < 6; i++)
        {
            var angle = Deg60InRadians * i + Deg30InRadians;

            points[i] = new HexPoint(
                center.X + tileSize * (float)Math.Cos(angle),
                center.Y + tileSize * (float)Math.Sin(angle)
            );
        }

        return points;
    }

    /// <summary>
    /// Returns the total pixel size required to render a grid
    /// of the given dimensions, including margins.
    /// </summary>
    public static HexSize GetGridPixelSize(int cols, int rows, float tileSize)
    {
        var hexWidth  = HexWidth(tileSize);
        var hexHeight = HexHeight(tileSize);
        var vertStep  = VerticalStep(tileSize);

        var width  = cols * hexWidth + hexWidth / 2f + tileSize * 2f;
        var height = rows * vertStep + hexHeight / 2f + hexHeight;

        return new HexSize(width, height);
    }

    // ── Geometry helpers (public for tests) ───────────────────

    /// <summary>Horizontal distance between adjacent hex centers.</summary>
    public static float HexWidth(float tileSize)
        => (float)(Math.Sqrt(3.0) * tileSize);

    /// <summary>Tip-to-tip vertical height of a hex.</summary>
    public static float HexHeight(float tileSize)
        => 2f * tileSize;

    /// <summary>Vertical distance between row centers (3/4 of hex height).</summary>
    public static float VerticalStep(float tileSize)
        => HexHeight(tileSize) * 0.75f;

    // ── Private helpers ───────────────────────────────────────

    private static bool IsOddRow(int row) => (row & 1) == 1;
}