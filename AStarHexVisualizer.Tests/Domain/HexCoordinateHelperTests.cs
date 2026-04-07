using AStarHexVisualizer.Domain;

namespace AStarHexVisualizer.Tests.Domain;

public class HexCoordinateHelperTests
{
    [Fact]
    public void GetNeighbor_CenterTile_ReturnsSixNeighbors()
    {
        var grid = new HexGrid(7, 7);
        var tile = grid.GetTile(3, 3);

        var neighbors = HexCoordinateHelper.GetNeighbors(tile!, grid).ToList();
        Assert.Equal(6, neighbors.Count);
    }
    [Fact]
    public void GetNeighbors_NoDuplicates()
    {
        // Given
        var grid = new HexGrid(7, 7);
        var tile = grid.GetTile(3, 3);
        // When
        var neighbors = HexCoordinateHelper.GetNeighbors(tile!, grid).Select(t => (t.Col, t.Row)).ToList();
        // Then
        Assert.Equal(neighbors.Count, neighbors.Distinct().Count());
    }
    [Fact]
    public void GetNeighbors_CornerTile_ReturnsFewerThanSix()
    {
        // Given
        var grid = new HexGrid(7, 7);
        var corner = grid.GetTile(0, 0);
        // When
        var neighbors = HexCoordinateHelper.GetNeighbors(corner!, grid).ToList();
        Console.WriteLine(neighbors.Count);
        // Then
        Assert.True(neighbors.Count < 6, $"Corner tile should have fewer than 6 neighbors, got {neighbors.Count}");
    }
    [Fact]
    public void GetNeighbors_TopEdge_NoNeighborOutOfBounds()
    {
        // Given
        var grid = new HexGrid(7, 7);
        var topEdgeTile = grid.GetTile(3, 0)!;
        // When
        var neighbors = HexCoordinateHelper.GetNeighbors(topEdgeTile, grid).ToList();
        // Then
        Assert.All(neighbors, n =>
        {
            Assert.InRange(n.Col, 0, 6);
            Assert.InRange(n.Row, 0, 6);
        });
    }
    [Fact]
    public void GetNeighbors_EvenRow_EastNeighborIsCorrect()
    {   // On an even row, East = (+1, 0)
        // Given
        var grid = new HexGrid(7, 7);
        var tile = grid.GetTile(3, 2)!; //row 2 = even

        var east = HexCoordinateHelper.GetNeighborInDirection(tile, HexDirection.East, grid);
        Assert.NotNull(east);
        Assert.Equal(4, east!.Col);
        Assert.Equal(2, east.Row);
    }
    [Fact]
    public void GetNeighbors_OddRow_NorthEastNeighborIsCorrect()
    {
        // On an odd row, NorthEast = (+1, -1)
        var grid = new HexGrid(7, 7);
        var tile = grid.GetTile(3, 3)!; // row 3 = odd

        var ne = HexCoordinateHelper
            .GetNeighborInDirection(tile, HexDirection.NorthEast, grid);

        Assert.NotNull(ne);
        Assert.Equal(4, ne!.Col);
        Assert.Equal(2, ne!.Row);
    }
    [Fact]
    public void GetNeighbors_EvenRow_NorthEastNeighborIsCorrect()
    {
        // On an even row, NorthEast = (0, -1)
        var grid = new HexGrid(7, 7);
        var tile = grid.GetTile(3, 2)!; // row 2 = even

        var ne = HexCoordinateHelper
            .GetNeighborInDirection(tile, HexDirection.NorthEast, grid);

        Assert.NotNull(ne);
        Assert.Equal(3, ne!.Col); // same col
        Assert.Equal(1, ne!.Row);
    }
    [Fact]
    public void GetNeighbors_NeighborContainsOriginalTile()
    {
        // If B is a neighbor of A, then A must be a neighbor of B
        var grid = new HexGrid(7, 7);
        var tileA = grid.GetTile(3, 3)!;

        var neighborsOfA = HexCoordinateHelper
            .GetNeighbors(tileA, grid).ToList();

        foreach (var tileB in neighborsOfA)
        {
            var neighborsOfB = HexCoordinateHelper
                .GetNeighbors(tileB, grid)
                .Select(t => (t.Col, t.Row));

            Assert.Contains((tileA.Col, tileA.Row), neighborsOfB);
        }
    }
}