using AStarHexVisualizer.Domain;

namespace AStarHexVisualizer.Tests.Domain;

public class MovementCostProviderTests
{
    // ── Cost values ───────────────────────────────────────────

    [Fact]
    public void GetCost_EmptyTile_ReturnsEmptyConstant()
    {
        
        var cost = MovementCostProvider.GetCost(TileType.Empty);

        Assert.Equal(MovementCosts.Empty, cost);
    }

    [Fact]
    public void GetCost_SlimeTile_ReturnsSlimeConstant()
    {
        var cost = MovementCostProvider.GetCost(TileType.Slime);

        Assert.Equal(MovementCosts.Slime, cost);
    }

    [Fact]
    public void GetCost_StartTile_ReturnsStartConstant()
    {
        var cost = MovementCostProvider.GetCost(TileType.Start);

        Assert.Equal(MovementCosts.Start, cost);
    }

    [Fact]
    public void GetCost_GoalTile_ReturnsGoalConstant()
    {
        var cost = MovementCostProvider.GetCost(TileType.Goal);

        Assert.Equal(MovementCosts.Goal, cost);
    }

    [Fact]
    public void GetCost_RockTile_ReturnsImpassable()
    {
        var cost = MovementCostProvider.GetCost(TileType.Rock);

        Assert.Equal(MovementCosts.Rock, cost);
    }

    [Fact]
    public void GetCost_RockTile_ReturnsDoubleMaxValue()
    {
        // Explicit check — double.MaxValue is the sentinel
        var cost = MovementCostProvider.GetCost(TileType.Rock);

        Assert.Equal(double.MaxValue, cost);
    }

    // ── Relative cost ordering ────────────────────────────────

    [Fact]
    public void GetCost_SlimeIsMoreExpensiveThanEmpty()
    {
        Assert.True(
            MovementCostProvider.GetCost(TileType.Slime) >
            MovementCostProvider.GetCost(TileType.Empty));
    }

    [Fact]
    public void GetCost_RockIsMoreExpensiveThanSlime()
    {
        Assert.True(
            MovementCostProvider.GetCost(TileType.Rock) >
            MovementCostProvider.GetCost(TileType.Slime));
    }

    // ── IsPassable ────────────────────────────────────────────

    [Theory]
    [InlineData(TileType.Empty, true)]
    [InlineData(TileType.Slime, true)]
    [InlineData(TileType.Start, true)]
    [InlineData(TileType.Goal,  true)]
    [InlineData(TileType.Rock,  false)]
    public void IsPassable_ReturnsCorrectValuePerTileType(
        TileType type,
        bool expected)
    {
        Assert.Equal(expected, MovementCostProvider.IsPassable(type));
    }

    // ── All TileType values are handled ───────────────────────

    [Fact]
    public void GetCost_AllTileTypesAreHandled()
    {
        // If a new TileType is ever added without updating
        // MovementCostProvider, this test will fail with an exception
        foreach (TileType type in Enum.GetValues<TileType>())
        {
            var ex = Record.Exception(
                () => MovementCostProvider.GetCost(type));

            Assert.Null(ex);
        }
    }
}