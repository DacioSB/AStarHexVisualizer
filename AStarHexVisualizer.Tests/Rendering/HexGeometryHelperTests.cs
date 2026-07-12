using AStarHexVisualizer.Rendering;

namespace AStarHexVisualizer.Tests.Rendering;

public class HexGeometryHelperTests
{
    private const float TileSize = 32f;
    private const float Tolerance = 0.01f; // pixel precision tolerance

    // ── GetHexCenter ──────────────────────────────────────────

    [Fact]
    public void GetHexCenter_Origin_IsOffsetByMargins()
    {
        // (0,0) center should be at (tileSize + tileSize, hexHeight/2)
        // i.e. left margin + half height
        var center = HexGeometryHelper.GetHexCenter(0, 0, TileSize);

        Assert.True(center.X > 0, "Center X must be positive (has left margin)");
        Assert.True(center.Y > 0, "Center Y must be positive (has top margin)");
    }

    [Fact]
    public void GetHexCenter_SameRow_XIncreasesWithCol()
    {
        var c0 = HexGeometryHelper.GetHexCenter(0, 0, TileSize);
        var c1 = HexGeometryHelper.GetHexCenter(1, 0, TileSize);
        var c2 = HexGeometryHelper.GetHexCenter(2, 0, TileSize);

        Assert.True(c1.X > c0.X, "Col 1 center must be right of col 0");
        Assert.True(c2.X > c1.X, "Col 2 center must be right of col 1");
    }

    [Fact]
    public void GetHexCenter_SameRow_EqualHorizontalSpacing()
    {
        var c0 = HexGeometryHelper.GetHexCenter(0, 0, TileSize);
        var c1 = HexGeometryHelper.GetHexCenter(1, 0, TileSize);
        var c2 = HexGeometryHelper.GetHexCenter(2, 0, TileSize);

        var spacing01 = c1.X - c0.X;
        var spacing12 = c2.X - c1.X;

        Assert.Equal(spacing01, spacing12, Tolerance);
    }

    [Fact]
    public void GetHexCenter_SameRow_YIsConstant()
    {
        var c0 = HexGeometryHelper.GetHexCenter(0, 0, TileSize);
        var c1 = HexGeometryHelper.GetHexCenter(1, 0, TileSize);
        var c2 = HexGeometryHelper.GetHexCenter(2, 0, TileSize);

        Assert.Equal(c0.Y, c1.Y, Tolerance);
        Assert.Equal(c1.Y, c2.Y, Tolerance);
    }

    [Fact]
    public void GetHexCenter_SameCol_YIncreasesWithRow()
    {
        var c0 = HexGeometryHelper.GetHexCenter(0, 0, TileSize);
        var c1 = HexGeometryHelper.GetHexCenter(0, 1, TileSize);
        var c2 = HexGeometryHelper.GetHexCenter(0, 2, TileSize);

        Assert.True(c1.Y > c0.Y);
        Assert.True(c2.Y > c1.Y);
    }

    [Fact]
    public void GetHexCenter_OddRow_IsShiftedRightOfEvenRow()
    {
        // Odd rows shift right by hexWidth/2
        var evenRow = HexGeometryHelper.GetHexCenter(0, 0, TileSize); // row 0 = even
        var oddRow  = HexGeometryHelper.GetHexCenter(0, 1, TileSize); // row 1 = odd

        Assert.True(oddRow.X > evenRow.X,
            "Odd row col 0 should be shifted right of even row col 0");
    }

    [Fact]
    public void GetHexCenter_OddRowShift_EqualsHalfHexWidth()
    {
        var evenRow = HexGeometryHelper.GetHexCenter(0, 0, TileSize);
        var oddRow  = HexGeometryHelper.GetHexCenter(0, 1, TileSize);

        var expectedShift = HexGeometryHelper.HexWidth(TileSize) / 2f;
        var actualShift   = oddRow.X - evenRow.X;

        Assert.Equal(expectedShift, actualShift, Tolerance);
    }

    [Fact]
    public void GetHexCenter_VerticalSpacing_EqualsThreeQuartersHexHeight()
    {
        var c0 = HexGeometryHelper.GetHexCenter(0, 0, TileSize);
        var c1 = HexGeometryHelper.GetHexCenter(0, 1, TileSize);

        var expected = HexGeometryHelper.VerticalStep(TileSize);
        var actual   = c1.Y - c0.Y;

        Assert.Equal(expected, actual, Tolerance);
    }

    // ── GetHexPoints ──────────────────────────────────────────

    [Fact]
    public void GetHexPoints_ReturnsSixVertices()
    {
        var points = HexGeometryHelper.GetHexPoints(0, 0, TileSize);

        Assert.Equal(6, points.Length);
    }

    [Fact]
    public void GetHexPoints_AllVerticesAtCorrectDistanceFromCenter()
    {
        // All 6 vertices must be exactly tileSize away from center
        var center = HexGeometryHelper.GetHexCenter(3, 3, TileSize);
        var points = HexGeometryHelper.GetHexPoints(3, 3, TileSize);

        foreach (var p in points)
        {
            var dx       = p.X - center.X;
            var dy       = p.Y - center.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);

            Assert.Equal(TileSize, distance, Tolerance);
        }
    }

    [Fact]
    public void GetHexPoints_NoTwoDuplicateVertices()
    {
        var points = HexGeometryHelper.GetHexPoints(3, 3, TileSize);

        for (var i = 0; i < points.Length; i++)
        for (var j = i + 1; j < points.Length; j++)
        {
            var dx = points[i].X - points[j].X;
            var dy = points[i].Y - points[j].Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            Assert.True(dist > Tolerance,
                $"Vertices {i} and {j} are duplicates or too close");
        }
    }

    [Fact]
    public void GetHexPoints_PointyTop_TopVertexIsAboveCenter()
    {
        // Pointy-top: vertex at 90° (straight up) is above center
        var center = HexGeometryHelper.GetHexCenter(3, 3, TileSize);
        var points = HexGeometryHelper.GetHexPoints(3, 3, TileSize);

        // The topmost vertex should be above the center
        var topmost = points.MinBy(p => p.Y);
        Assert.True(topmost.Y < center.Y, "Pointy-top hex must have a vertex above center");
    }

    // ── Tiling — no gaps or overlaps ──────────────────────────

    [Fact]
    public void GetHexCenter_AdjacentCols_SpacingEqualsHexWidth()
    {
        // Adjacent tiles in same row must be exactly hexWidth apart
        var expected = HexGeometryHelper.HexWidth(TileSize);

        for (var row = 0; row < 10; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                var c0 = HexGeometryHelper.GetHexCenter(col,     row, TileSize);
                var c1 = HexGeometryHelper.GetHexCenter(col + 1, row, TileSize);

                Assert.Equal(expected, c1.X - c0.X, Tolerance);
            }
        }
    }

    [Fact]
    public void GetHexCenter_AdjacentRows_SpacingEqualsVerticalStep()
    {
        // Adjacent rows must be exactly vertStep apart vertically
        var expected = HexGeometryHelper.VerticalStep(TileSize);

        for (var col = 0; col < 10; col++)
        {
            for (var row = 0; row < 9; row++)
            {
                var c0 = HexGeometryHelper.GetHexCenter(col, row,     TileSize);
                var c1 = HexGeometryHelper.GetHexCenter(col, row + 1, TileSize);

                Assert.Equal(expected, c1.Y - c0.Y, Tolerance);
            }
        }
    }

    // ── GetGridPixelSize ──────────────────────────────────────

    [Fact]
    public void GetGridPixelSize_ReturnsPositiveDimensions()
    {
        var size = HexGeometryHelper.GetGridPixelSize(10, 10, TileSize);

        Assert.True(size.Width  > 0);
        Assert.True(size.Height > 0);
    }

    [Fact]
    public void GetGridPixelSize_LargerGrid_ProducesLargerSize()
    {
        var small = HexGeometryHelper.GetGridPixelSize(5,  5,  TileSize);
        var large = HexGeometryHelper.GetGridPixelSize(10, 10, TileSize);

        Assert.True(large.Width  > small.Width);
        Assert.True(large.Height > small.Height);
    }

    // ── Geometry constants ────────────────────────────────────

    [Fact]
    public void HexWidth_IsSquareRootThreeTimesTileSize()
    {
        var expected = (float)(Math.Sqrt(3.0) * TileSize);
        Assert.Equal(expected, HexGeometryHelper.HexWidth(TileSize), Tolerance);
    }

    [Fact]
    public void HexHeight_IsTwoTimesTileSize()
    {
        Assert.Equal(2f * TileSize, HexGeometryHelper.HexHeight(TileSize), Tolerance);
    }

    [Fact]
    public void VerticalStep_IsThreeQuartersOfHexHeight()
    {
        var expected = HexGeometryHelper.HexHeight(TileSize) * 0.75f;
        Assert.Equal(expected, HexGeometryHelper.VerticalStep(TileSize), Tolerance);
    }
}