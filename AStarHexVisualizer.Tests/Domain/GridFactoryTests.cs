using AStarHexVisualizer.Algorithm;
using AStarHexVisualizer.Domain;

namespace AStarHexVisualizer.Tests.Domain;

public class GridFactoryTests
{
    // ── Common invariants across all scenarios ─────────────────

    [Theory]
    [InlineData(ScenarioType.SimpleOpen)]
    [InlineData(ScenarioType.RockMaze)]
    [InlineData(ScenarioType.SlimePenalty)]
    public void Create_AllScenarios_HaveCorrectDimensions(ScenarioType scenario)
    {
        var grid = GridFactory.Create(scenario);

        Assert.Equal(GridFactory.GridSize, grid.Cols);
        Assert.Equal(GridFactory.GridSize, grid.Rows);
    }

    [Theory]
    [InlineData(ScenarioType.SimpleOpen)]
    [InlineData(ScenarioType.RockMaze)]
    [InlineData(ScenarioType.SlimePenalty)]
    public void Create_AllScenarios_HaveExactlyOneStart(ScenarioType scenario)
    {
        var grid = GridFactory.Create(scenario);

        var startCount = grid.GetAllTiles()
            .Count(t => t.Type == TileType.Start);

        Assert.Equal(1, startCount);
    }

    [Theory]
    [InlineData(ScenarioType.SimpleOpen)]
    [InlineData(ScenarioType.RockMaze)]
    [InlineData(ScenarioType.SlimePenalty)]
    public void Create_AllScenarios_HaveExactlyOneGoal(ScenarioType scenario)
    {
        var grid = GridFactory.Create(scenario);

        var goalCount = grid.GetAllTiles()
            .Count(t => t.Type == TileType.Goal);

        Assert.Equal(1, goalCount);
    }

    [Theory]
    [InlineData(ScenarioType.SimpleOpen)]
    [InlineData(ScenarioType.RockMaze)]
    [InlineData(ScenarioType.SlimePenalty)]
    public void Create_AllScenarios_StartIsAtTopLeft(ScenarioType scenario)
    {
        var grid = GridFactory.Create(scenario);

        Assert.NotNull(grid.StartTile);
        Assert.Equal(0, grid.StartTile!.Col);
        Assert.Equal(0, grid.StartTile!.Row);
    }

    [Theory]
    [InlineData(ScenarioType.SimpleOpen)]
    [InlineData(ScenarioType.RockMaze)]
    [InlineData(ScenarioType.SlimePenalty)]
    public void Create_AllScenarios_GoalIsAtBottomRight(ScenarioType scenario)
    {
        var grid = GridFactory.Create(scenario);
        var expected = GridFactory.GridSize - 1;

        Assert.NotNull(grid.GoalTile);
        Assert.Equal(expected, grid.GoalTile!.Col);
        Assert.Equal(expected, grid.GoalTile!.Row);
    }

    [Theory]
    [InlineData(ScenarioType.SimpleOpen)]
    [InlineData(ScenarioType.RockMaze)]
    [InlineData(ScenarioType.SlimePenalty)]
    public void Create_AllScenarios_StartAndGoalAreNeverObstacles(ScenarioType scenario)
    {
        var grid = GridFactory.Create(scenario);

        Assert.NotEqual(TileType.Rock,  grid.StartTile!.Type);
        Assert.NotEqual(TileType.Slime, grid.StartTile!.Type);
        Assert.NotEqual(TileType.Rock,  grid.GoalTile!.Type);
        Assert.NotEqual(TileType.Slime, grid.GoalTile!.Type);
    }

    [Theory]
    [InlineData(ScenarioType.SimpleOpen)]
    [InlineData(ScenarioType.RockMaze)]
    [InlineData(ScenarioType.SlimePenalty)]
    public void Create_EachCall_ReturnsNewInstance(ScenarioType scenario)
    {
        var gridA = GridFactory.Create(scenario);
        var gridB = GridFactory.Create(scenario);

        Assert.NotSame(gridA, gridB);
    }

    // ── SimpleOpen ────────────────────────────────────────────

    [Fact]
    public void Create_SimpleOpen_HasNoRocksOrSlime()
    {
        var grid = GridFactory.Create(ScenarioType.SimpleOpen);

        var obstacles = grid.GetAllTiles()
            .Where(t => t.Type is TileType.Rock or TileType.Slime);

        Assert.Empty(obstacles);
    }

    // ── RockMaze ──────────────────────────────────────────────

    [Fact]
    public void Create_RockMaze_HasRocks()
    {
        var grid = GridFactory.Create(ScenarioType.RockMaze);

        var rocks = grid.GetAllTiles()
            .Where(t => t.Type == TileType.Rock);

        Assert.NotEmpty(rocks);
    }

    [Fact]
    public void Create_RockMaze_WallExistsAtExpectedColumn()
    {
        var grid = GridFactory.Create(ScenarioType.RockMaze);

        // Vertical wall at col 3, rows 2-7
        for (var row = 2; row <= 7; row++)
            Assert.Equal(TileType.Rock, grid.GetTile(3, row)!.Type);
    }

    [Fact]
    public void Create_RockMaze_GapExistsAtRowEight()
    {
        var grid = GridFactory.Create(ScenarioType.RockMaze);

        // The gap — col 3 row 8 must NOT be a rock
        Assert.NotEqual(TileType.Rock, grid.GetTile(3, 8)!.Type);
    }

    [Fact]
    public void Create_RockMaze_IsStillSolvable()
    {
        var grid   = GridFactory.Create(ScenarioType.RockMaze);
        var engine = new AStarEngine();
        engine.Initialize(grid, grid.StartTile!, grid.GoalTile!);

        StepResult result;
        do { result = engine.Step(); } while (!result.IsFinished);

        Assert.True(result.PathFound,
            "RockMaze must always have at least one valid path to the goal.");
    }

    // ── SlimePenalty ──────────────────────────────────────────

    [Fact]
    public void Create_SlimePenalty_HasSlimeTiles()
    {
        var grid = GridFactory.Create(ScenarioType.SlimePenalty);

        var slime = grid.GetAllTiles()
            .Where(t => t.Type == TileType.Slime);

        Assert.NotEmpty(slime);
    }

    [Fact]
    public void Create_SlimePenalty_SlimeRunsDiagonally()
    {
        var grid = GridFactory.Create(ScenarioType.SlimePenalty);

        // Spot-check several diagonal positions
        Assert.Equal(TileType.Slime, grid.GetTile(1, 1)!.Type);
        Assert.Equal(TileType.Slime, grid.GetTile(4, 4)!.Type);
        Assert.Equal(TileType.Slime, grid.GetTile(7, 7)!.Type);
    }

    [Fact]
    public void Create_SlimePenalty_IsStillSolvable()
    {
        var grid   = GridFactory.Create(ScenarioType.SlimePenalty);
        var engine = new AStarEngine();
        engine.Initialize(grid, grid.StartTile!, grid.GoalTile!);

        StepResult result;
        do { result = engine.Step(); } while (!result.IsFinished);

        Assert.True(result.PathFound,
            "SlimePenalty must always have a valid path to the goal.");
    }

    // ── Unknown scenario ──────────────────────────────────────

    [Fact]
    public void Create_UnknownScenario_ThrowsArgumentOutOfRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => GridFactory.Create((ScenarioType)999));
    }
}