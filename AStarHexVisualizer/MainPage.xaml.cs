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
}
