using AStarHexVisualizer.Algorithm;
using AStarHexVisualizer.Domain;
using AStarHexVisualizer.Rendering;
using AStarHexVisualizer.ViewModels;

namespace AStarHexVisualizer;

public partial class MainPage : ContentPage
{
    private const ScenarioType DefaultScenario = ScenarioType.RockMaze;
	private readonly HexGridDrawable _drawable = new();
    private readonly AStarEngine     _engine   = new();
    private HexGrid?                 _grid;

    private int _stepCount;

	public MainPage()
	{
		InitializeComponent();

        var scenarios = ScenarioInfo.All.ToList();
        ScenarioPicker.ItemsSource   = scenarios;
        ScenarioPicker.SelectedIndex = scenarios.FindIndex(s => s.Type == DefaultScenario);

        LoadScenario(DefaultScenario);

        ScenarioPicker.SelectedIndexChanged += OnScenarioChanged;
	}
    private void OnScenarioChanged(object? sender, EventArgs e)
    {
        if (ScenarioPicker.SelectedItem is not ScenarioInfo info)
            return;

        LoadScenario(info.Type);
    }
	private void LoadScenario(ScenarioType scenario)
    {
        _grid = GridFactory.Create(scenario);

        _stepCount = 0;

        _engine.Initialize(_grid, _grid.StartTile!, _grid.GoalTile!);
        _drawable.Grid = _grid;
        _drawable.CurrentTile = null;
        ScenarioDescriptionLabel.Text = ScenarioInfo.For(scenario).Description;
        
        UpdateTileSize();

        HexCanvas.Drawable = _drawable;
        HexCanvas.Invalidate();

        UpdateInfoPanel();
    }

    // ── Canvas scaling ────────────────────────────────────────

    private void OnCanvasSizeChanged(object? sender, EventArgs e)
    {
        UpdateTileSize();
        HexCanvas.Invalidate();
    }

    /// <summary>
    /// Finds the largest tile size where the full grid
    /// still fits inside the current canvas dimensions.
    /// </summary>
    private void UpdateTileSize()
    {
        if (_grid is null || HexCanvas.Height <= 0 || HexCanvas.Width <= 0)
            return;

        const float padding = 16f;

        var availableW = (float)HexCanvas.Width  - padding * 2;
        var availableH = (float)HexCanvas.Height - padding * 2;

        if (availableW <= 0 || availableH <= 0)
            return;

        // Task 2: largest tile size that fits both axes
        var tileSize = CanvasLayoutHelper.ComputeBestTileSize(
            _grid.Cols, _grid.Rows, availableW, availableH);

        // Task 1: exact drawn size at that tile size
        var gridSize = HexGeometryHelper.GetGridPixelSize(
            _grid.Cols, _grid.Rows, tileSize);

        // Task 3: center the grid, balancing slack on both axes
        var (offsetX, offsetY) = CanvasLayoutHelper.ComputeCenteringOffsets(
            gridSize.Width, gridSize.Height, availableW, availableH, padding);

        _drawable.TileSize = tileSize;
        _drawable.OffsetX  = offsetX;
        _drawable.OffsetY  = offsetY;
    }

    private void UpdateInfoPanel()
    {

        StepCountLabel.Text = _stepCount == 0 ? "-" : _stepCount.ToString();
        OpenListLabel.Text = _engine.OpenList.Count.ToString();
        ClosedListLabel.Text = _engine.ClosedList.Count.ToString();

        if (_engine.IsFinished)
        {
            StatusLabel.Text = _engine.PathFound ? "Path Found! ✓" : "No Path ✗";
            StatusLabel.TextColor = _engine.PathFound ? Color.FromArgb("#27AE60") : Color.FromArgb("#E74C3C");
        } else
        {
            StatusLabel.Text = _stepCount == 0 ? "Ready" : "Searching...";
            StatusLabel.TextColor = Color.FromArgb("#A0A8C0");
        }

    }
}
