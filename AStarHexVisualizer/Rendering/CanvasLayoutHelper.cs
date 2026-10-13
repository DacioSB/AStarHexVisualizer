//namespace
namespace AStarHexVisualizer.Rendering;

/// <summary>
/// Pure math helper for pointy-top hexagon geometry on an odd-r offset grid.
/// </summary>
public static class CanvasLayoutHelper
{
    /// <summary>
    /// Centers a grid of pixel size (gridW x gridH) inside an available
    /// area, returning the (offsetX, offsetY) to pass to the drawable.
    /// The base padding is added on top of the centering slack.
    /// </summary>
    public static (float offsetX, float offsetY) ComputeCenteringOffsets(
        float gridW, float gridH,
        float availableW, float availableH,
        float padding)
    {
        var slackX = MathF.Max(0f, (availableW - gridW) / 2f);
        var slackY = MathF.Max(0f, (availableH - gridH) / 2f);

        return (padding + slackX, padding + slackY);
    }

    public static float ComputeBestTileSize(
            int cols, int rows,
            float availableW, float availableH,
            float minTile = 4f,
            float maxTile = 400f,
            float step = 0.5f
        )
    {
        if (cols <= 0 || rows <= 0 || availableW <= 0 || availableH <= 0)
            return minTile;

        var best = minTile;

        for (var candidate = minTile; candidate <= maxTile; candidate += step)
        {
            var size = HexGeometryHelper.GetGridPixelSize(cols, rows, candidate);

            if (size.Width <= availableW && size.Height <= availableH)
                best = candidate; 
            else
                break;
        }

        return best;
    }
}