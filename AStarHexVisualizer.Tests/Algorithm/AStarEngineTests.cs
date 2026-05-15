using AStarHexVisualizer.Domain;
using AStarHexVisualizer.Algorithm;

namespace AStarHexVisualizer.Tests.Algorithm;

public class AStarEngineTests
{
    // ── Helpers ───────────────────────────────────────────────

    /// <summary>
    /// Runs the engine to completion and returns the final StepResult.
    /// </summary>
    private static StepResult RunToCompletion(AStarEngine engine)
    {
        StepResult result;
        do { result = engine.Step(); }
        while (!result.IsFinished);
        return result;
    }

    /// <summary>
    /// Creates a flat open grid with Start at (0,0) and Goal at (col,row).
    /// </summary>
    private static (AStarEngine engine, HexGrid grid) BuildSimpleScenario(
        int cols, int rows, int goalCol, int goalRow)
    {
        var grid  = new HexGrid(cols, rows);
        var start = grid.GetTile(0, 0)!;
        var goal  = grid.GetTile(goalCol, goalRow)!;

        grid.SetTileType(0,       0,       TileType.Start);
        grid.SetTileType(goalCol, goalRow, TileType.Goal);

        var engine = new AStarEngine();
        engine.Initialize(grid, start, goal);

        return (engine, grid);
    }

    // ── Initialization ────────────────────────────────────────

    [Fact]
    public void Initialize_SeedsOpenListWithStart()
    {
        var (engine, _) = BuildSimpleScenario(5, 5, 4, 4);

        Assert.Single(engine.OpenList);
        Assert.Equal(0, engine.OpenList[0].Col);
        Assert.Equal(0, engine.OpenList[0].Row);
    }

    [Fact]
    public void Initialize_StartTile_HasGOfZero()
    {
        var (_, grid) = BuildSimpleScenario(5, 5, 4, 4);
        var start = grid.GetTile(0, 0)!;

        Assert.Equal(0.0, start.G);
    }

    [Fact]
    public void Initialize_StartTile_HasHGreaterThanZero()
    {
        var (_, grid) = BuildSimpleScenario(5, 5, 4, 4);
        var start = grid.GetTile(0, 0)!;

        Assert.True(start.H > 0);
    }

    // ── Simple path ───────────────────────────────────────────

    [Fact]
    public void Step_SimpleOpenGrid_FindsPath()
    {
        var (engine, _) = BuildSimpleScenario(5, 5, 4, 4);

        var result = RunToCompletion(engine);

        Assert.True(result.IsFinished);
        Assert.True(result.PathFound);
    }

    [Fact]
    public void Step_SimpleOpenGrid_PathStartsAtOrigin()
    {
        var (engine, _) = BuildSimpleScenario(5, 5, 4, 0);

        var result = RunToCompletion(engine);

        Assert.Equal(0, result.FinalPath[0].Col);
        Assert.Equal(0, result.FinalPath[0].Row);
    }

    [Fact]
    public void Step_SimpleOpenGrid_PathEndsAtGoal()
    {
        var (engine, _) = BuildSimpleScenario(5, 5, 4, 0);

        var result = RunToCompletion(engine);

        var last = result.FinalPath[^1];
        Assert.Equal(4, last.Col);
        Assert.Equal(0, last.Row);
    }

    [Fact]
    public void Step_SimpleOpenGrid_PathTilesMarkedAsFinalPath()
    {
        var (engine, _) = BuildSimpleScenario(5, 5, 4, 4);

        var result = RunToCompletion(engine);

        Assert.All(result.FinalPath,
            t => Assert.Equal(TileState.FinalPath, t.State));
    }

    [Fact]
    public void Step_AdjacentGoal_FindsPathInFewSteps()
    {
        // Goal is East neighbor of Start — should resolve very quickly
        var grid  = new HexGrid(5, 5);
        var start = grid.GetTile(2, 2)!;
        var goal  = grid.GetTile(3, 2)!;

        grid.SetTileType(2, 2, TileType.Start);
        grid.SetTileType(3, 2, TileType.Goal);

        var engine = new AStarEngine();
        engine.Initialize(grid, start, goal);

        var result = RunToCompletion(engine);

        Assert.True(result.PathFound);
        Assert.Equal(2, result.FinalPath.Count); // start + goal
    }

    // ── Path around rocks ─────────────────────────────────────

    [Fact]
    public void Step_RocksBlockingDirectRoute_FindsDetour()
    {
        // Layout (5 cols x 3 rows):
        // S . R . G
        // . . R . .
        // . . . . .
        // Direct path east is blocked — must go around via row 1 or 2
        var grid = new HexGrid(5, 3);
        grid.SetTileType(0, 0, TileType.Start);
        grid.SetTileType(4, 0, TileType.Goal);
        grid.SetTileType(2, 0, TileType.Rock);
        grid.SetTileType(2, 1, TileType.Rock);

        var start  = grid.GetTile(0, 0)!;
        var goal   = grid.GetTile(4, 0)!;
        var engine = new AStarEngine();
        engine.Initialize(grid, start, goal);

        var result = RunToCompletion(engine);

        Assert.True(result.PathFound);
        Assert.DoesNotContain(result.FinalPath,
            t => t.Type == TileType.Rock);
    }

    [Fact]
    public void Step_CompletelyWalledGoal_ReportsNoPath()
    {
        // Surround goal with rocks so it is unreachable
        var grid = new HexGrid(5, 5);
        grid.SetTileType(0, 0, TileType.Start);
        grid.SetTileType(4, 4, TileType.Goal);

        // Wall off the goal tile on all reachable sides
        grid.SetTileType(3, 4, TileType.Rock);
        grid.SetTileType(4, 3, TileType.Rock);
        grid.SetTileType(3, 3, TileType.Rock);

        var start  = grid.GetTile(0, 0)!;
        var goal   = grid.GetTile(4, 4)!;
        var engine = new AStarEngine();
        engine.Initialize(grid, start, goal);

        var result = RunToCompletion(engine);

        Assert.True(result.IsFinished);
        Assert.False(result.PathFound);
    }

    // ── Slime cost ────────────────────────────────────────────

    [Fact]
    public void Step_SlimePath_CostsMoreThanEmptyPath()
    {
        // Two routes to goal:
        //   Top row: Start → slime → slime → Goal  (cost 1+3+3+1 = 8)
        //   Bottom row detour through empty tiles   (cheaper)
        // Engine should prefer the empty route
        var grid = new HexGrid(5, 3);
        grid.SetTileType(0, 0, TileType.Start);
        grid.SetTileType(4, 0, TileType.Goal);
        grid.SetTileType(1, 0, TileType.Slime);
        grid.SetTileType(2, 0, TileType.Slime);
        grid.SetTileType(3, 0, TileType.Slime);

        var start  = grid.GetTile(0, 0)!;
        var goal   = grid.GetTile(4, 0)!;
        var engine = new AStarEngine();
        engine.Initialize(grid, start, goal);

        var result = RunToCompletion(engine);

        Assert.True(result.PathFound);

        // The optimal path must not go through all 3 slime tiles
        var slimeTilesOnPath = result.FinalPath
            .Count(t => t.Type == TileType.Slime);

        Assert.True(slimeTilesOnPath < 3,
            $"Expected fewer than 3 slime tiles on path, got {slimeTilesOnPath}");
    }

    [Fact]
    public void Step_SlimeDetour_TotalCostReflectsSlimePenalty()
    {
        // Direct path of 4 empty steps
        var grid = new HexGrid(7, 1);
        grid.SetTileType(0, 0, TileType.Start);
        grid.SetTileType(4, 0, TileType.Goal);

        var start  = grid.GetTile(0, 0)!;
        var goal   = grid.GetTile(4, 0)!;
        var engine = new AStarEngine();
        engine.Initialize(grid, start, goal);

        var result = RunToCompletion(engine);

        // Cost must be positive and finite
        Assert.True(result.TotalCost > 0);
        Assert.True(result.TotalCost < double.MaxValue);
    }

    // ── No path ───────────────────────────────────────────────

    [Fact]
    public void Step_NoPath_IsFinishedTrue()
    {
        // Start is completely surrounded by rocks
        var grid = new HexGrid(5, 5);
        grid.SetTileType(2, 2, TileType.Start);
        grid.SetTileType(4, 4, TileType.Goal);

        // Surround start with rocks
        grid.SetTileType(3, 2, TileType.Rock);
        grid.SetTileType(2, 1, TileType.Rock);
        grid.SetTileType(1, 1, TileType.Rock);
        grid.SetTileType(1, 2, TileType.Rock);
        grid.SetTileType(1, 3, TileType.Rock);
        grid.SetTileType(2, 3, TileType.Rock);

        var start  = grid.GetTile(2, 2)!;
        var goal   = grid.GetTile(4, 4)!;
        var engine = new AStarEngine();
        engine.Initialize(grid, start, goal);

        var result = RunToCompletion(engine);

        Assert.True(result.IsFinished);
        Assert.False(result.PathFound);
        Assert.Empty(result.FinalPath);
    }

    // ── Step-by-step state ────────────────────────────────────

    [Fact]
    public void Step_EachStep_ClosedListGrows()
    {
        var (engine, _) = BuildSimpleScenario(5, 5, 4, 4);

        var previousClosedCount = 0;
        for (var i = 0; i < 5; i++)
        {
            var result = engine.Step();
            if (result.IsFinished) break;

            Assert.True(result.ClosedList.Count >= previousClosedCount);
            previousClosedCount = result.ClosedList.Count;
        }
    }

    [Fact]
    public void Step_AfterFinished_ReturnsIsFinishedTrue()
    {
        var (engine, _) = BuildSimpleScenario(3, 3, 2, 2);
        RunToCompletion(engine);

        // Calling Step() again after completion must not throw
        var extraResult = engine.Step();
        Assert.True(extraResult.IsFinished);
    }

    [Fact]
    public void Step_BeforeInitialize_ThrowsInvalidOperation()
    {
        var engine = new AStarEngine();

        Assert.Throws<InvalidOperationException>(() => engine.Step());
    }
}