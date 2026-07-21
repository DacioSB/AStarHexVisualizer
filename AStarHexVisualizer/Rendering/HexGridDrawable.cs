using Microsoft.Maui.Graphics;
using AStarHexVisualizer.Domain;
using AStarHexVisualizer.Algorithm;

namespace AStarHexVisualizer.Rendering;

/// <summary>
/// MAUI IDrawable that renders the full hex grid each frame.
///
/// Rendering pipeline per tile:
///   1. Fill hexagon with state-encoded color
///   2. Stroke border (highlighted if current step tile)
///   3. Draw label: emoji for rocks/slime, S/G for start/goal,
///      f/g/h values for open and closed tiles
///
/// Called by a GraphicsView — Draw() is invoked every Invalidate().
/// </summary>
public class HexGridDrawable : IDrawable
{
    // ── Configuration ─────────────────────────────────────────
    public float TileSize     { get; set; } = HexGeometryHelper.DefaultTileSize;
    public float OffsetX      { get; set; } = 0f;
    public float OffsetY      { get; set; } = 0f;

    // ── Data ──────────────────────────────────────────────────
    public HexGrid?   Grid        { get; set; }
    public HexTile?   CurrentTile { get; set; }  // tile being evaluated this step

    // ── Font sizes ────────────────────────────────────────────
    private const float LabelFontSize  = 13f;  // S, G, emoji
    private const float FValueFontSize = 11f;  // f(n) value — prominent
    private const float GHFontSize     =  8f;  // g and h — small supporting info

    // ── IDrawable ─────────────────────────────────────────────

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Grid is null) return;

        // Fill canvas background
        canvas.FillColor = TileColors.Canvas;
        canvas.FillRectangle(dirtyRect);

        foreach (var tile in Grid.GetAllTiles())
            DrawTile(canvas, tile);
    }

    // ── Tile rendering ────────────────────────────────────────

    private void DrawTile(ICanvas canvas, HexTile tile)
    {
        var center = HexGeometryHelper.GetHexCenter(
            tile.Col, tile.Row, TileSize, OffsetX, OffsetY);

        var points = HexGeometryHelper.GetHexPointsFromCenter(center, TileSize);
        var isCurrentTile = tile == CurrentTile;

        DrawHexFill(canvas, points, tile);
        DrawHexStroke(canvas, points, tile, isCurrentTile);
        DrawTileLabel(canvas, center, tile);
    }

    // ── Fill ──────────────────────────────────────────────────

    private static void DrawHexFill(ICanvas canvas, HexPoint[] points, HexTile tile)
    {
        canvas.FillColor = TileColors.GetFill(tile);

        var path = BuildHexPath(points);
        canvas.FillPath(path);
    }

    // ── Stroke ────────────────────────────────────────────────

    private static void DrawHexStroke(
        ICanvas canvas,
        HexPoint[] points,
        HexTile tile,
        bool isCurrentTile)
    {
        canvas.StrokeColor = TileColors.GetStroke(tile, isCurrentTile);
        canvas.StrokeSize  = isCurrentTile ? 2.5f : 1f;

        var path = BuildHexPath(points);
        canvas.DrawPath(path);
    }

    // ── Labels ────────────────────────────────────────────────

    private void DrawTileLabel(ICanvas canvas, HexPoint center, HexTile tile)
    {
        // Start and Goal — always show their letter regardless of state
        if (tile.Type == TileType.Start)
        {
            DrawCenteredText(canvas, center, "S",
                TileColors.TextPrimary, LabelFontSize, bold: true);
            return;
        }

        if (tile.Type == TileType.Goal)
        {
            DrawCenteredText(canvas, center, "G",
                TileColors.TextPrimary, LabelFontSize, bold: true);
            return;
        }

        // Rocks — emoji label, no values
        if (tile.Type == TileType.Rock)
        {
            DrawCenteredText(canvas, center, "🪨",
                TileColors.TextPrimary, LabelFontSize, bold: false);
            return;
        }

        // Unvisited tiles — show slime emoji only
        if (tile.State == TileState.Unvisited)
        {
            if (tile.Type == TileType.Slime)
                DrawCenteredText(canvas, center, "🟢",
                    TileColors.TextPrimary, LabelFontSize, bold: false);
            return;
        }

        // Open, Closed, FinalPath — show f/g/h values
        DrawAStarValues(canvas, center, tile);
    }

    /// <summary>
    /// Draws the A* values inside the hex as a mini scoreboard:
    ///   f value — large, centered
    ///   g | h   — small, bottom-left and bottom-right
    /// </summary>
    private void DrawAStarValues(ICanvas canvas, HexPoint center, HexTile tile)
    {
        var textColor = tile.IsOnFinalPath
            ? TileColors.TextOnPath
            : TileColors.TextPrimary;

        var mutedColor = tile.IsOnFinalPath
            ? TileColors.TextOnPath
            : TileColors.TextMuted;

        // f value — prominent center label
        var fText = tile.G >= double.MaxValue / 2
            ? "∞"
            : $"{tile.F:F0}";

        DrawCenteredText(canvas, center, fText,
            textColor, FValueFontSize, bold: true,
            yOffset: -TileSize * 0.1f);

        // g and h — small labels below
        if (tile.G < double.MaxValue / 2)
        {
            var gText = $"g:{tile.G:F0}";
            var hText = $"h:{tile.H:F0}";

            var yBase   = center.Y + TileSize * 0.25f;
            var xLeft   = center.X - TileSize * 0.35f;
            var xRight  = center.X + TileSize * 0.05f;

            DrawText(canvas, xLeft,  yBase, gText, mutedColor, GHFontSize);
            DrawText(canvas, xRight, yBase, hText, mutedColor, GHFontSize);
        }
    }

    // ── Text helpers ──────────────────────────────────────────

    private static void DrawCenteredText(
        ICanvas canvas,
        HexPoint center,
        string text,
        Color color,
        float fontSize,
        bool bold,
        float yOffset = 0f)
    {
        canvas.FontColor = color;
        canvas.FontSize  = fontSize;

        canvas.DrawString(
            text,
            center.X - TextBoxWidth(fontSize),
            center.Y - fontSize / 2f + yOffset,
            TextBoxWidth(fontSize) * 2f,
            fontSize * 1.5f,
            HorizontalAlignment.Center,
            VerticalAlignment.Center
        );
    }

    private static void DrawText(
        ICanvas canvas,
        float x, float y,
        string text,
        Color color,
        float fontSize)
    {
        canvas.FontColor = color;
        canvas.FontSize  = fontSize;

        canvas.DrawString(
            text,
            x, y,
            60f, fontSize * 1.5f,
            HorizontalAlignment.Left,
            VerticalAlignment.Top
        );
    }

    // Approximates a reasonable text box width based on font size
    private static float TextBoxWidth(float fontSize) => fontSize * 3.5f;

    // ── Path builder ──────────────────────────────────────────

    /// <summary>
    /// Builds a closed PathF from 6 HexPoints.
    /// Converts from our own HexPoint to MAUI's PointF at the boundary.
    /// </summary>
    private static PathF BuildHexPath(HexPoint[] points)
    {
        var path = new PathF();
        path.MoveTo(points[0].X, points[0].Y);

        for (var i = 1; i < points.Length; i++)
            path.LineTo(points[i].X, points[i].Y);

        path.Close();
        return path;
    }
}