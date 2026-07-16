namespace AStarHexVisualizer.Rendering;

/// <summary>
/// Centralized color definitions for all tile states.
/// All rendering decisions about color live here — nowhere else.
/// Dark theme: charcoal canvas, information-encoded colors.
/// </summary>
public static class TileColors
{
    // ── Canvas ────────────────────────────────────────────────
    public static readonly Color Canvas        = Color.FromArgb("#1A1A2E");
    public static readonly Color TileStroke    = Color.FromArgb("#2D2D44");

    // ── Tile fill colors ──────────────────────────────────────
    public static readonly Color Unvisited     = Color.FromArgb("#2A2A3E");
    public static readonly Color Rock          = Color.FromArgb("#4A3728");
    public static readonly Color SlimeUnvisited = Color.FromArgb("#2D4A2D");
    public static readonly Color SlimeActive   = Color.FromArgb("#3A6B2A");
    public static readonly Color OpenList      = Color.FromArgb("#1B4F7A");
    public static readonly Color ClosedList    = Color.FromArgb("#0D3352");
    public static readonly Color FinalPath     = Color.FromArgb("#D4A017");
    public static readonly Color Start         = Color.FromArgb("#27AE60");
    public static readonly Color Goal          = Color.FromArgb("#E74C3C");

    // ── Text colors ───────────────────────────────────────────
    public static readonly Color TextPrimary   = Color.FromArgb("#E8E8F0");
    public static readonly Color TextMuted     = Color.FromArgb("#A0A8C0");
    public static readonly Color TextOnPath    = Color.FromArgb("#1A1A2E");

    // ── Highlight (current step) ──────────────────────────────
    public static readonly Color ActiveStroke  = Color.FromArgb("#FFFFFF");
    public static readonly Color PathStroke    = Color.FromArgb("#F0C040");
}