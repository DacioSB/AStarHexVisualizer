using AStarHexVisualizer.Domain;
namespace AStarHexVisualizer.Tests.Domain;

public class HexTileTests
{
    [Fact]
    public void HexTile_DefaultInitialization_HasCorrectPosition()
    {
        var tile = new HexTile(3, 5);

        Assert.Equal(3, tile.Col);
        Assert.Equal(5, tile.Row);
    }
    [Fact]
    public void HexTile_DefaultInitialization_TypeIsEmpty()
    {
        var tile = new HexTile(0, 0);

        Assert.Equal(TileType.Empty, tile.Type);
    }
    [Fact]
    public void HexTile_DefaultInitialization_GIsMaxValue()
    {
        var tile = new HexTile(0, 0);

        Assert.Equal(double.MaxValue, tile.G);
    }
    [Fact]
    public void HexTile_DefaultInitialization_StateIsUnvisited()
    {
        var tile = new HexTile(0, 0);

        Assert.Equal(TileState.Unvisited, tile.State);
    }
    [Fact]
    public void HexTile_DefaultInitialization_ParentIsNull()
    {
        var tile = new HexTile(0, 0);

        Assert.Null(tile.Parent);
    }
    [Fact]
    public void HexTile_F_IsComputedFromGandH()
    {
        var tile = new HexTile(0, 0) { G = 4.0, H = 3.0 };

        Assert.Equal(7.0, tile.F);
    }
    [Fact]
    public void HexTile_F_UpdatesWhenGChanges()
    {
        var tile = new HexTile(0, 0) { G = 2.0, H = 5.0 };
        tile.G = 10.0;

        Assert.Equal(15.0, tile.F);
    }
    [Theory]
    [InlineData(TileType.Empty, true)]
    [InlineData(TileType.Slime, true)]
    [InlineData(TileType.Start, true)]
    [InlineData(TileType.Goal,  true)]
    [InlineData(TileType.Rock,  false)]
    public void HexTile_IsWalkable_CorrectPerTileType(TileType type, bool expected)
    {
        var tile = new HexTile(0, 0, type);

        Assert.Equal(expected, tile.IsWalkable);
    }
    [Theory]
    [InlineData(TileType.Empty, MovementCosts.Empty)]
    [InlineData(TileType.Slime, MovementCosts.Slime)]
    [InlineData(TileType.Start, MovementCosts.Start)]
    [InlineData(TileType.Goal,  MovementCosts.Goal)]
    public void HexTile_MovementCost_MatchesConstants(TileType type, double expectedCost)
    {
        var tile = new HexTile(0, 0, type);

        Assert.Equal(expectedCost, tile.MovementCost);
    }
    [Fact]
    public void HexTile_MovementCost_RockIsMaxValue()
    {
        var tile = new HexTile(0, 0, TileType.Rock);

        Assert.Equal(double.MaxValue, tile.MovementCost);
    }
    [Fact]
    public void HexTile_Reset_ClearsGHParentAndState()
    {
        var parent = new HexTile(1, 1);
        var tile   = new HexTile(0, 0)
        {
            G           = 5.0,
            H           = 3.0,
            Parent      = parent,
            State       = TileState.Closed,
            IsVisited   = true,
            IsInOpenList  = true,
            IsOnFinalPath = true
        };

        tile.ResetState();

        Assert.Equal(double.MaxValue, tile.G);
        Assert.Equal(0,               tile.H);
        Assert.Null(tile.Parent);
        Assert.Equal(TileState.Unvisited, tile.State);
        Assert.False(tile.IsVisited);
        Assert.False(tile.IsInOpenList);
        Assert.False(tile.IsOnFinalPath);
    }
    [Fact]
    public void HexTile_Reset_PreservesTileTypeAndPosition()
    {
        var tile = new HexTile(2, 4, TileType.Slime)
        {
            G = 99
        };

        tile.ResetState();

        // Terrain and position must survive a reset
        Assert.Equal(TileType.Slime, tile.Type);
        Assert.Equal(2, tile.Col);
        Assert.Equal(4, tile.Row);
    }
}