namespace AStarHexVisualizer.Domain;

/// <summary>
/// Defines what a tile's current STATUS is during A* execution.
/// This drives the visual rendering color/highlight.
/// </summary>
public enum TileState
{
    Unvisited,
    Open,
    Closed,
    FinalPath
}