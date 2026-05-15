using AStarHexVisualizer.Domain;

namespace AStarHexVisualizer.Algorithm;

/// <summary>
/// Step-by-step A* pathfinding engine for a hex grid.
///
/// Usage:
///   1. Call Initialize(grid, start, goal)
///   2. Call Step() repeatedly until StepResult.IsFinished is true
///   3. If StepResult.PathFound, StepResult.FinalPath holds the route
///
/// One Step() call = one tile popped from the open list and evaluated.
/// This granularity lets the visualizer render each iteration separately.
/// </summary>
public class AStarEngine
{
    // ── State ─────────────────────────────────────────────────

    private HexGrid?   _grid;
    private HexTile?   _goal;

    private readonly List<HexTile> _openList   = [];
    private readonly List<HexTile> _closedList = [];

    private bool _isInitialized;
    private bool _isFinished;
    private bool _pathFound;

    // ── Public read-only views ────────────────────────────────

    public IReadOnlyList<HexTile> OpenList   => _openList;
    public IReadOnlyList<HexTile> ClosedList => _closedList;
    public bool IsFinished => _isFinished;
    public bool PathFound  => _pathFound;

    // ── Initialize ────────────────────────────────────────────

    /// <summary>
    /// Prepares the engine for a new search.
    /// Resets all algorithm state on the grid and seeds the open list
    /// with the start tile.
    /// </summary>
    public void Initialize(HexGrid grid, HexTile start, HexTile goal)
    {
        _grid  = grid;
        _goal  = goal;

        _openList.Clear();
        _closedList.Clear();

        _isFinished    = false;
        _pathFound     = false;
        _isInitialized = true;

        // Reset all tile algorithm state
        grid.ResetAlgorithmState();

        // Seed the open list with the start tile
        start.G           = 0;
        start.H           = HeuristicCalculator.Calculate(start, goal);
        start.State       = TileState.Open;
        start.IsInOpenList = true;

        _openList.Add(start);
    }

    // ── Step ──────────────────────────────────────────────────

    /// <summary>
    /// Advances the algorithm by exactly one iteration.
    /// Returns a StepResult snapshot for the visualizer to render.
    /// </summary>
    public StepResult Step()
    {
        if (!_isInitialized)
            throw new InvalidOperationException(
                "Call Initialize() before Step().");

        // Already done — return the terminal state
        if (_isFinished)
            return BuildResult(currentTile: null, updatedTiles: []);

        // ── No path exists ────────────────────────────────────
        if (_openList.Count == 0)
        {
            _isFinished = true;
            _pathFound  = false;
            return BuildResult(currentTile: null, updatedTiles: []);
        }

        // ── Pop lowest-F tile ─────────────────────────────────
        var current = PopLowestCost();
        var updatedTiles = new List<HexTile> { current };

        // Move to closed list
        current.IsInOpenList = false;
        current.IsVisited    = true;
        current.State        = TileState.Closed;
        _closedList.Add(current);

        // ── Goal check ────────────────────────────────────────
        if (current == _goal)
        {
            _isFinished = true;
            _pathFound  = true;

            var path = ReconstructPath(current);
            return BuildResult(current, updatedTiles, path);
        }

        // ── Evaluate neighbors ────────────────────────────────
        var neighbors = HexCoordinateHelper
            .GetNeighbors(current, _grid!)
            .Where(n => !n.IsVisited)
            .Where(n => MovementCostProvider.IsPassable(n.Type));

        foreach (var neighbor in neighbors)
        {
            var tentativeG = current.G
                + MovementCostProvider.GetCost(neighbor.Type);

            // Found a cheaper route to this neighbor
            if (tentativeG < neighbor.G)
            {
                neighbor.Parent      = current;
                neighbor.G           = tentativeG;
                neighbor.H           = HeuristicCalculator
                                          .Calculate(neighbor, _goal!);
                neighbor.State       = TileState.Open;
                neighbor.IsInOpenList = true;

                if (!_openList.Contains(neighbor))
                    _openList.Add(neighbor);

                updatedTiles.Add(neighbor);
            }
        }

        // Keep open list sorted by F ascending
        _openList.Sort((a, b) => a.F.CompareTo(b.F));

        return BuildResult(current, updatedTiles);
    }

    // ── Path Reconstruction ───────────────────────────────────

    /// <summary>
    /// Traces parent pointers from goal back to start.
    /// Marks each tile on the path with TileState.FinalPath.
    /// Returns tiles ordered from start → goal.
    /// </summary>
    private static List<HexTile> ReconstructPath(HexTile goal)
    {
        var path = new List<HexTile>();
        var current = goal;

        while (current is not null)
        {
            current.IsOnFinalPath = true;
            current.State         = TileState.FinalPath;
            path.Add(current);
            current = current.Parent;
        }

        path.Reverse(); // start → goal order
        return path;
    }

    // ── Helpers ───────────────────────────────────────────────

    /// <summary>
    /// Removes and returns the tile with the lowest F from the open list.
    /// List is kept sorted, so this is always the first element.
    /// </summary>
    private HexTile PopLowestCost()
    {
        var tile = _openList[0];
        _openList.RemoveAt(0);
        return tile;
    }

    /// <summary>
    /// Builds a StepResult from current engine state.
    /// </summary>
    private StepResult BuildResult(
        HexTile? currentTile,
        List<HexTile> updatedTiles,
        List<HexTile>? finalPath = null)
    {
        return new StepResult
        {
            CurrentTile  = currentTile,
            OpenList     = [.._openList],
            ClosedList   = [.._closedList],
            UpdatedTiles = updatedTiles,
            FinalPath    = finalPath ?? [],
            IsFinished   = _isFinished,
            PathFound    = _pathFound,
            TotalCost    = finalPath is not null && _goal is not null
                               ? _goal.G
                               : 0
        };
    }
}