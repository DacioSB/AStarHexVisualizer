namespace AStarHexVisualizer.Rendering;

/// <summary>
/// Pure math helper for pointy-top hexagon geometry on an odd-r offset grid.
///
/// Coordinate conventions:
///   tileSize = circumradius R (center to vertex, in pixels)
///   hexWidth  = sqrt(3) * R   (horizontal spacing between centers)
///   hexHeight = 2 * R         (tip-to-tip vertical height)
///   vertStep  = hexHeight * 3/4 (vertical distance between row centers)
///
/// Odd rows are shifted right by hexWidth/2 to produce the hex tiling.
///
/// No MAUI or UI dependencies — all inputs and outputs are plain floats.
/// PointF is from System.Drawing (available in .NET without MAUI).
/// </summary>
public static class HexGeometryHelper
{
    // ── Constants ─────────────────────────────────────────────

    /// <summary>Default tile size (circumradius in pixels).</summary>
    public const float DefaultTileSize = 32f;

    private const double Deg30InRadians = Math.PI / 6.0;   // 30°
    private const double Deg60InRadians = Math.PI / 3.0;   // 60°

    // ── Public API ────────────────────────────────────────────
    /// <summary>
    /// Returns the screen-space center point of the hex tile at (col, row).
    /// </summary>
    /// <param name="col">Grid column (0-based).</param>
    /// <param name="row">Grid row (0-based).</param>
    /// <param name="tileSize">Circumradius in pixels.</param>
    /// <param name="offsetX">Optional left padding in pixels.</param>
    /// <param name="offsetY">Optional top padding in pixels.</param>
    public static PointF GetHexCenter(int col, int row, float tileSize, float offsetX = 0f, float offsetY = 0f)
    {
        var hexW = HexWidth(tileSize);
        var hexH = HexHeight(tileSize);
        var vStep = VerticalStep(tileSize);

        float cx = col * hexW + (IsOddRow(row) ? hexW / 2f : 0f) + offsetX + tileSize;
        float cy = row * vStep + offsetY + hexH / 2f;
        return new PointF(cx, cy);
    }

    // ── Geometry helpers (public for tests) ───────────────────

    /// <summary>Horizontal distance between adjacent hex centers.</summary>
    public static float HexWidth(float tileSize) => (float)(Math.Sqrt(3) * tileSize);
    /// <summary>Tip-to-tip vertical height of a hex.</summary>
    public static float HexHeight(float tileSize) => 2f * tileSize;
    /// <summary>Vertical distance between row centers (3/4 of hex height).</summary>
    public static float VerticalStep(float tileSize) => HexHeight(tileSize) * 0.75f;
    // ── Private helpers ───────────────────────────────────────

    private static bool IsOddRow(int row) => (row & 1) == 1;
}