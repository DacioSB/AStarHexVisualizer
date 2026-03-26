using AStarHexVisualizer.Domain;

namespace AStarHexVisualizer.Tests.Domain;

public class HexGridTests
{
    [Fact]
    public void HexGrid_Initialization_HasCorrectDimensions()
    {
        var grid = new HexGrid(8, 10);

        Assert.Equal(8,  grid.Cols);
        Assert.Equal(10, grid.Rows);
    }
    [Fact]
    public void HexGrid_Initialization_AllTilesAreEmpty()
    {
        var grid = new HexGrid(5, 5);

        Assert.All(grid.GetAllTiles(), t => Assert.Equal(TileType.Empty, t.Type));
    }
    [Fact]
    public void HexGrid_Initialization_StartAndGoalAreNull()
    {
        var grid = new HexGrid(5, 5);

        Assert.Null(grid.StartTile);
        Assert.Null(grid.GoalTile);
    }
    [Fact]
    public void HexGrid_GetTile_ReturnsCorrectTile()
    {
        var grid = new HexGrid(5, 5);
        var tile = grid.GetTile(2, 3);

        Assert.NotNull(tile);
        Assert.Equal(2, tile.Col);
        Assert.Equal(3, tile.Row);
    }
    [Theory]
    [InlineData(-1,  0)]
    [InlineData( 0, -1)]
    [InlineData( 5,  0)]
    [InlineData( 0,  5)]
    public void HexGrid_GetTile_ReturnsNullWhenOutOfBounds(int col, int row)
    {
        var grid = new HexGrid(5, 5);

        Assert.Null(grid.GetTile(col, row));
    }

}