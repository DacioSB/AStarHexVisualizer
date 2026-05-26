namespace AStarHexVisualizer.Domain;

/// <summary>
/// Named preset scenarios available in the GridFactory.
/// Each value maps to a hand-crafted grid demonstrating
/// a different aspect of A* behavior.
/// </summary>
public enum ScenarioType
{
    /// <summary>
    /// Open field with no obstacles.
    /// Shows pure A* propagation expanding toward the goal.
    /// </summary>
    SimpleOpen,

    /// <summary>
    /// A wall of rocks with a single gap forces a visible detour.
    /// Shows A* navigating around impassable terrain.
    /// </summary>
    RockMaze,

    /// <summary>
    /// The direct path is covered in slime (cost 3).
    /// A* finds the cheaper route around the edges.
    /// Shows cost-based decision making over raw distance.
    /// </summary>
    SlimePenalty
}