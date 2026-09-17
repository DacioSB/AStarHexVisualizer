using AStarHexVisualizer.Algorithm;
using AStarHexVisualizer.Domain;
using AStarHexVisualizer.Rendering;

namespace AStarHexVisualizer;

public partial class MainPage : ContentPage
{
	private readonly HexGridDrawable _drawable = new();
    private readonly AStarEngine     _engine   = new();
    private HexGrid?                 _grid;

	public MainPage()
	{
		InitializeComponent();
        LoadScenario(ScenarioType.RockMaze);
	}
	private void LoadScenario(ScenarioType scenario)
    {
        _grid = GridFactory.Create(scenario);

        _drawable.Grid     = _grid;
        _drawable.TileSize = 32f;
        _drawable.OffsetX  = 8f;
        _drawable.OffsetY  = 8f;

        HexCanvas.Drawable = _drawable;
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
}
