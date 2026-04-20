using System.Runtime.Intrinsics.Arm;
using AStarHexVisualizer.Domain;

namespace AStarHexVisualizer.Algorithm;

/// <summary>
/// Calculates the h(n) heuristic for the A* algorithm.
///
/// Uses hex cube distance, which is admissible (never overestimates)
/// because it represents the minimum possible number of steps
/// between two tiles on an unobstructed hex grid.
///
/// Internally converts odd-r offset coordinates to cube coordinates
/// for accurate hex distance calculation. The public API stays in
/// offset space — callers never deal with cube coordinates.
/// </summary>
public static class HeuristicCalculator
{
    /// <summary>
    /// Returns the estimated cost h(n) from current to goal.
    /// Uses hex cube distance — admissible and consistent.
    ///
    /// Pure function: no state is read or modified.
    /// </summary>
    public static Double Calculate(HexTile start, HexTile goal)
    {
        var (sq, sr, ss) = ToCube(start.Col, start.Row);
        var (gq, gr, gs) = ToCube(goal.Col, goal.Row);

        return (Math.Abs(sq - gq) + Math.Abs(sr - gr) + Math.Abs(ss - gs)) / 2.0;
    }

    private static (int q, int r, int s) ToCube(int col, int row)
    {
        int q = col - (row - (row & 1)) / 2;
        int r = row;
        int s = -q - r;
        return (q, r, s);
    }
}