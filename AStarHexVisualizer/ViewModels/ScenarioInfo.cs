using AStarHexVisualizer.Domain;

namespace AStarHexVisualizer.ViewModels;

/// <summary>
/// Presentation data for a ScenarioType: what the user sees in the picker.
/// Pure C# — no MAUI dependency, so it is unit-testable.
/// </summary>
public sealed record ScenarioInfo(ScenarioType Type, string Name, string Description)
{
    public static IReadOnlyList<ScenarioInfo> All { get; } = [
        new(ScenarioType.SimpleOpen,
            "Simple Open",
            "No obstacles. Watch the search expand in a wave toward the goal."),

        new(ScenarioType.RockMaze,
            "Rock Maze",
            "A rock wall forces a detour through the gap."),

        new(ScenarioType.SlimePenalty,
            "Slime Penalty",
            "Slime costs 3 to cross. The cheapest route avoids the direct path.")
    ];

    /// <summary>Returns the info entry for a scenario type.</summary>
    public static ScenarioInfo For(ScenarioType type)
    {
        return All.First((s) => s.Type == type);
    }
    /// <summary>Picker uses ToString() as the display text.</summary>
    public override string ToString() => Name;
}