using AStarHexVisualizer.Algorithm;
using AStarHexVisualizer.Domain;
using AStarHexVisualizer.Rendering;

namespace AStarHexVisualizer;

public partial class MainPage : ContentPage
{
	private readonly HexGridDrawable _drawable = new();
    private readonly AStarEngine     _engine   = new();
    private HexGrid?                 _grid;

    private int _stepCount;

	public MainPage()
	{
		InitializeComponent();
        LoadScenario(ScenarioType.RockMaze);
	}
	private void LoadScenario(ScenarioType scenario)
    {
        _grid = GridFactory.Create(scenario);

        _stepCount = 0;

        //engine init
        //drawable grid is the grid we created
        //da um update no grid

        _engine.Initialize(_grid, _grid.StartTile!, _grid.GoalTile!);
        _drawable.Grid = _grid;
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
        {
            return;
        }
        // the visible part of the tiles grid will have 16px of padding in all directions
        //so the available size is the width - 16px*2 (right and left) height - 16px*2 (up and down)
        var padding = 16f;
        var availableW = (float) HexCanvas.Width - padding * 2;
        var availableH = (float) HexCanvas.Height - padding * 2;

        var tileSize = 10f;
        for (float candidate = 10f; candidate < 80f; candidate += 1f)
        {
            var size = HexGeometryHelper.GetGridPixelSize(_grid.Cols, _grid.Rows, candidate);

            if (size.Width <= availableW && size.Height <= availableH)
            {
                tileSize = candidate;
            }
            else
            {
                break;
            }
        }

        _drawable.TileSize = tileSize;
        _drawable.OffsetX = padding;
        _drawable.OffsetY = padding;
    }

    private void UpdateInfoPanel()
    {
        //primeiro digo que StepCountLabel é "-" se o stepcount for 0, se não, eu coloco o valor do stepcount.tostring
        //OpenListLabel e ClosedListLabel e setar para o tamanho da lista de abertos e fechados na engine (tostring)
        //se engine is finished e achou o goal, então StatusLabel texto vai ser Path Found! ✓ e cor #27AE60
        //se não "No Path ✗" e #E74C3C
        //se nao ta finished, ou step count é zero e eu coloco statuslabel para Ready ou ta procurando Searching…
        //Color.FromArgb(#A0A8C0)

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
