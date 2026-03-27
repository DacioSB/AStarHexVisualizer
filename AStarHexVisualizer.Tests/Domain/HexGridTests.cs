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
    [Fact]
    public void HexGrid_SetTileType_TracksStartTile()
    {
        var grid = new HexGrid(5, 5);
        grid.SetTileType(1, 1, TileType.Start);

        Assert.NotNull(grid.StartTile);
        Assert.Equal(1, grid.StartTile!.Col);
        Assert.Equal(1, grid.StartTile!.Row);
    }
    [Fact]
    public void HexGrid_SetTileType_TracksGoalTile()
    {
        var grid = new HexGrid(5, 5);
        grid.SetTileType(4, 4, TileType.Goal);

        Assert.NotNull(grid.GoalTile);
        Assert.Equal(4, grid.GoalTile!.Col);
        Assert.Equal(4, grid.GoalTile!.Row);
    }
    [Fact]
    public void HexGrid_SetTileType_ClearsStartRefWhenOverwritten()
    {
        var grid = new HexGrid(5, 5);
        grid.SetTileType(0, 0, TileType.Start);
        grid.SetTileType(0, 0, TileType.Empty); // overwrite

        Assert.Null(grid.StartTile);
    }
    [Fact]
    public void HexGrid_AllTiles_ReturnsCorrectCount()
    {
        var grid = new HexGrid(6, 4);

        Assert.Equal(24, grid.GetAllTiles().Count());
    }
    [Fact]
    public void HexGrid_Reset_ClearsAlgorithmStateOnAllTiles()
    {
        var grid = new HexGrid(4, 4);
        foreach (var tile in grid.GetAllTiles())
        {
            tile.G     = 99;
            tile.State = TileState.Closed;
        }

        grid.ResetAlgorithmState();

        Assert.All(grid.GetAllTiles(), t =>
        {
            Assert.Equal(double.MaxValue,  t.G);
            Assert.Equal(TileState.Unvisited, t.State);
        });
    }
    [Fact]
    public void HexGrid_Reset_PreservesTileTypes()
    {
        var grid = new HexGrid(4, 4);
        grid.SetTileType(1, 1, TileType.Rock);
        grid.SetTileType(2, 2, TileType.Slime);

        grid.ResetAlgorithmState();

        Assert.Equal(TileType.Rock,  grid.GetTile(1, 1)!.Type);
        Assert.Equal(TileType.Slime, grid.GetTile(2, 2)!.Type);
    }

}