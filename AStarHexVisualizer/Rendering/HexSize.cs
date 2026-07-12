namespace AStarHexVisualizer.Rendering;

/// <summary>
/// A lightweight 2D size with single-precision dimensions.
/// Used by HexGeometryHelper to avoid any MAUI or System.Drawing dependency.
/// </summary>
public readonly struct HexSize(float width, float height)
{
    public float Width  { get; } = width;
    public float Height { get; } = height;

    public override string ToString() => $"{Width:F2} x {Height:F2}";
}