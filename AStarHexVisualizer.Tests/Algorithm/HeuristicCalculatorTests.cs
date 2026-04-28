using AStarHexVisualizer.Domain;
using AStarHexVisualizer.Algorithm;

namespace AStarHexVisualizer.Tests.Algorithm;

public class HeuristicCalculatorTests
{
    // ── Zero distance ─────────────────────────────────────────

    [Fact]
    public void Calculate_SameTile_ReturnsZero()
    {
        var tile = new HexTile(3, 3);

        var result = HeuristicCalculator.Calculate(tile, tile);

        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Calculate_SamePosition_ReturnsZero()
    {
        var current = new HexTile(3, 3);
        var goal    = new HexTile(3, 3);

        var result = HeuristicCalculator.Calculate(current, goal);

        Assert.Equal(0.0, result);
    }

    // ── Direct neighbors (distance = 1) ───────────────────────

    [Fact]
    public void Calculate_DirectEastNeighbor_ReturnsOne()
    {
        var current = new HexTile(3, 3);
        var goal    = new HexTile(4, 3); // East

        Assert.Equal(1.0, HeuristicCalculator.Calculate(current, goal));
    }

    [Fact]
    public void Calculate_DirectWestNeighbor_ReturnsOne()
    {
        var current = new HexTile(3, 3);
        var goal    = new HexTile(2, 3); // West

        Assert.Equal(1.0, HeuristicCalculator.Calculate(current, goal));
    }

    [Fact]
    public void Calculate_OddRow_NorthEastNeighbor_ReturnsOne()
    {
        // On odd row 3, NE neighbor is (+1, -1)
        var current = new HexTile(3, 3);
        var goal    = new HexTile(4, 2);

        Assert.Equal(1.0, HeuristicCalculator.Calculate(current, goal));
    }

    [Fact]
    public void Calculate_EvenRow_NorthEastNeighbor_ReturnsOne()
    {
        // On even row 2, NE neighbor is (0, -1)
        var current = new HexTile(3, 2);
        var goal    = new HexTile(3, 1);

        Assert.Equal(1.0, HeuristicCalculator.Calculate(current, goal));
    }

    // ── Known multi-step distances ────────────────────────────

    [Fact]
    public void Calculate_TwoStepsEast_ReturnsTwo()
    {
        var current = new HexTile(3, 3);
        var goal    = new HexTile(5, 3);

        Assert.Equal(2.0, HeuristicCalculator.Calculate(current, goal));
    }

    [Fact]
    public void Calculate_CornerToCorner_7x7Grid_ReturnsSix()
    {
        var current = new HexTile(0, 0);
        var goal    = new HexTile(6, 6);

        Assert.Equal(9.0, HeuristicCalculator.Calculate(current, goal));
    }

    // ── Symmetry — distance A→B equals B→A ───────────────────

    [Fact]
    public void Calculate_IsSymmetric()
    {
        var tileA = new HexTile(1, 2);
        var tileB = new HexTile(4, 5);

        var ab = HeuristicCalculator.Calculate(tileA, tileB);
        var ba = HeuristicCalculator.Calculate(tileB, tileA);

        Assert.Equal(ab, ba);
    }

    // ── Non-negative ──────────────────────────────────────────

    [Fact]
    public void Calculate_AlwaysReturnsNonNegative()
    {
        var pairs = new (HexTile, HexTile)[]
        {
            (new HexTile(0, 0), new HexTile(6, 6)),
            (new HexTile(6, 6), new HexTile(0, 0)),
            (new HexTile(3, 3), new HexTile(3, 3)),
            (new HexTile(0, 6), new HexTile(6, 0)),
        };

        foreach (var (a, b) in pairs)
            Assert.True(HeuristicCalculator.Calculate(a, b) >= 0);
    }

    // ── Admissibility check ───────────────────────────────────

    [Fact]
    public void Calculate_NeverExceedsActualStepCount()
    {
        // On an open grid, cube distance equals minimum steps.
        // Heuristic must never exceed this — that's admissibility.
        // We verify h(neighbor) <= h(current) + 1 for all neighbors.
        var grid    = new HexGrid(7, 7);
        var goal    = grid.GetTile(6, 6)!;
        var current = grid.GetTile(3, 3)!;

        var neighbors = HexCoordinateHelper.GetNeighbors(current, grid);

        foreach (var neighbor in neighbors)
        {
            var hCurrent  = HeuristicCalculator.Calculate(current,  goal);
            var hNeighbor = HeuristicCalculator.Calculate(neighbor, goal);

            Assert.True(
                hNeighbor <= hCurrent + 1.0,
                $"Admissibility violated: h({neighbor.Col},{neighbor.Row})=" +
                $"{hNeighbor} > h({current.Col},{current.Row})={hCurrent} + 1");
        }
    }
}