using Godot;

namespace JunkyardAutomation;

/// <summary>
/// Main - Root node script for the main game scene.
/// Sets up and coordinates all game systems.
/// </summary>
public partial class Main : Node2D
{
    private YardState _yardState = new();

    // Node references
    private CameraController? _camera;
    private GridRenderer? _gridRenderer;
    private TileHighlighter? _tileHighlighter;
    private PlacementManager? _placementManager;
    private PlacementGhost? _placementGhost;
    private ConveyorSystem? _conveyorSystem;
    private Node2D? _machinesContainer;
    private Node2D? _itemsContainer;
    private CanvasLayer? _uiLayer;
    private BuildMenu? _buildMenu;
    private DebugOverlay? _debugOverlay;

    // Autoloads
    private GridSystem? _gridSystem;
    private SimulationManager? _simulationManager;
    private ContentRegistry? _contentRegistry;
    private GameManager? _gameManager;

    public override void _Ready()
    {
        GD.Print("Main: Setting up scene...");

        // Get autoloads
        _gridSystem = GetNode<GridSystem>("/root/GridSystem");
        _simulationManager = GetNode<SimulationManager>("/root/SimulationManager");
        _contentRegistry = GetNode<ContentRegistry>("/root/ContentRegistry");
        _gameManager = GetNode<GameManager>("/root/GameManager");

        // Initialize yard state
        _yardState.Initialize(_gridSystem, _contentRegistry);

        // Set up scene hierarchy
        SetupCamera();
        SetupGrid();
        SetupContainers();
        SetupPlacement();
        SetupSystems();
        SetupUI();

        GD.Print("Main: Scene setup complete");
    }

    private void SetupCamera()
    {
        _camera = new CameraController();
        _camera.Name = "Camera";
        AddChild(_camera);
        _camera.MakeCurrent();
    }

    private void SetupGrid()
    {
        _gridRenderer = new GridRenderer();
        _gridRenderer.Name = "Grid";
        AddChild(_gridRenderer);

        _tileHighlighter = new TileHighlighter();
        _tileHighlighter.Name = "TileHighlighter";
        _gridRenderer.AddChild(_tileHighlighter);
    }

    private void SetupContainers()
    {
        _machinesContainer = new Node2D();
        _machinesContainer.Name = "Machines";
        AddChild(_machinesContainer);

        _itemsContainer = new Node2D();
        _itemsContainer.Name = "Items";
        AddChild(_itemsContainer);
    }

    private void SetupPlacement()
    {
        _placementGhost = new PlacementGhost();
        _placementGhost.Name = "PlacementGhost";
        AddChild(_placementGhost);

        _placementManager = new PlacementManager();
        _placementManager.Name = "PlacementManager";
        AddChild(_placementManager);

        _placementManager.Initialize(_yardState, _tileHighlighter!, _placementGhost, _machinesContainer!);
    }

    private void SetupSystems()
    {
        _conveyorSystem = new ConveyorSystem();
        _conveyorSystem.Name = "ConveyorSystem";
        AddChild(_conveyorSystem);
        _conveyorSystem.Initialize(_yardState, _gridSystem!);

        // Register with simulation manager
        _simulationManager?.RegisterSystem("conveyor", _conveyorSystem);
    }

    private void SetupUI()
    {
        _uiLayer = new CanvasLayer();
        _uiLayer.Name = "UI";
        AddChild(_uiLayer);

        // Build menu
        _buildMenu = new BuildMenu();
        _buildMenu.Name = "BuildMenu";
        _buildMenu.Position = new Vector2(10, 100);
        _uiLayer.AddChild(_buildMenu);

        _buildMenu.MachineSelected += OnMachineSelected;
        _buildMenu.DemolishSelected += OnDemolishSelected;

        // Debug overlay
        _debugOverlay = new DebugOverlay();
        _debugOverlay.Name = "DebugOverlay";
        _uiLayer.AddChild(_debugOverlay);
        _debugOverlay.SetTileHighlighter(_tileHighlighter!);
    }

    private void OnMachineSelected(string machineTypeId)
    {
        _placementManager?.EnterBuildMode(machineTypeId);
    }

    private void OnDemolishSelected()
    {
        _placementManager?.EnterDemolishMode();
    }

    public override void _Input(InputEvent @event)
    {
        // Debug: spawn item on space
        if (@event.IsActionPressed("debug_spawn_item"))
        {
            SpawnDebugItem();
        }
    }

    private void SpawnDebugItem()
    {
        if (_tileHighlighter == null || _gridSystem == null) return;

        var gridPos = _tileHighlighter.CurrentGridPos;
        var machine = _yardState.GetMachineAt(gridPos);

        if (machine != null && machine.TypeId == "conveyor")
        {
            var item = new ItemEntity
            {
                Id = _yardState.GenerateItemId(),
                TypeId = "scrap_ferrous",
                GridPosition = gridPos,
                TileProgress = 0,
                Direction = machine.Rotation
            };

            _yardState.AddItem(item);

            // Create visual
            var visual = new Sprite2D();
            visual.Texture = CreateItemTexture();
            visual.Position = _gridSystem.GridToWorld(gridPos);
            _itemsContainer?.AddChild(visual);
            item.Visual = visual;

            GD.Print($"Spawned item {item.Id} at {gridPos}");
        }
        else
        {
            GD.Print("Cannot spawn item - no conveyor at cursor position");
        }
    }

    private static ImageTexture CreateItemTexture()
    {
        var image = Image.CreateEmpty(16, 16, false, Image.Format.Rgba8);

        // Draw a simple circle
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                float dx = x - 8;
                float dy = y - 8;
                if (dx * dx + dy * dy <= 36)
                {
                    image.SetPixel(x, y, new Color(0.8f, 0.5f, 0.2f));
                }
            }
        }

        return ImageTexture.CreateFromImage(image);
    }
}
