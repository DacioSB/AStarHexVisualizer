namespace AStarHexVisualizer.Rendering;

/// <summary>
/// A lightweight 2D point with single-precision coordinates.
/// Used by HexGeometryHelper to avoid any MAUI or System.Drawing dependency.
/// </summary>
public readonly struct HexPoint(float x, float y)
{
    public float X { get; } = x;
    public float Y { get; } = y;

    public override string ToString() => $"({X:F2}, {Y:F2})";
}