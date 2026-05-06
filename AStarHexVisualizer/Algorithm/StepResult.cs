using AStarHexVisualizer.Domain;

namespace AStarHexVisualizer.Algorithm;

/// <summary>
/// Snapshot of the A* algorithm state after one Step() call.
/// The visualizer renders exactly this — one frame per step.
/// </summary>
public class StepResult
{
    /// <summary>
    /// The tile that was popped and evaluated in this step.
    /// Null only before the first step.
    /// </summary>
    public HexTile? CurrentTile { get; init; }

    /// <summary>
    /// All tiles currently in the open list after this step.
    /// </summary>
    public IReadOnlyList<HexTile> OpenList { get; init; } = [];

    /// <summary>
    /// All tiles currently in the closed list after this step.
    /// </summary>
    public IReadOnlyList<HexTile> ClosedList { get; init; } = [];

    /// <summary>
    /// Tiles whose state changed during this step (newly opened,
    /// cost updated, or moved to closed). Used for highlighting.
    /// </summary>
    public IReadOnlyList<HexTile> UpdatedTiles { get; init; } = [];

    /// <summary>
    /// Tiles on the final reconstructed path.
    /// Populated only when PathFound is true.
    /// </summary>
    public IReadOnlyList<HexTile> FinalPath { get; init; } = [];

    /// <summary>
    /// True when the algorithm has nothing left to do —
    /// either the goal was reached or no path exists.
    /// </summary>
    public bool IsFinished { get; init; }

    /// <summary>
    /// True if the goal was reached successfully.
    /// Meaningful only when IsFinished is true.
    /// </summary>
    public bool PathFound { get; init; }

    /// <summary>
    /// Total cost of the final path (sum of G at goal tile).
    /// Meaningful only when PathFound is true.
    /// </summary>
    public double TotalCost { get; init; }
}