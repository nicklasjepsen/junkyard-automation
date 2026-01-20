using Godot;

namespace JunkyardAutomation;

/// <summary>
/// GameManager - Main game state and initialization coordinator.
/// Handles game lifecycle, state management, and coordinates between systems.
/// </summary>
public partial class GameManager : Node
{
    [Signal]
    public delegate void GameInitializedEventHandler();

    [Signal]
    public delegate void GameStateChangedEventHandler(GameState newState);

    public enum GameState
    {
        Initializing,
        Loading,
        Playing,
        Paused,
        Menu
    }

    public GameState CurrentState { get; private set; } = GameState.Initializing;

    /// <summary>Reference to the main scene root</summary>
    public Node? MainScene { get; set; }

    // Autoload references (populated after ready)
    private GridSystem? _gridSystem;
    private SimulationManager? _simulationManager;
    private ContentRegistry? _contentRegistry;

    public override void _Ready()
    {
        // Initialize game on next frame to ensure all autoloads are ready
        CallDeferred(MethodName.InitializeGame);
    }

    private void InitializeGame()
    {
        GD.Print("GameManager: Initializing...");

        // Get autoload references
        _gridSystem = GetNode<GridSystem>("/root/GridSystem");
        _simulationManager = GetNode<SimulationManager>("/root/SimulationManager");
        _contentRegistry = GetNode<ContentRegistry>("/root/ContentRegistry");

        // Load content definitions
        ChangeState(GameState.Loading);
        _contentRegistry.LoadContent();

        GD.Print("GameManager: Content loaded");
        GD.Print($"  Items: {_contentRegistry.Items.Count}");
        GD.Print($"  Machines: {_contentRegistry.Machines.Count}");
        GD.Print($"  Recipes: {_contentRegistry.Recipes.Count}");

        ChangeState(GameState.Playing);
        EmitSignal(SignalName.GameInitialized);

        // Start simulation
        _simulationManager.Resume();
        GD.Print("GameManager: Game initialized and running");
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        EmitSignal(SignalName.GameStateChanged, (int)newState);
    }

    /// <summary>
    /// Pause the game.
    /// </summary>
    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
            _simulationManager?.Pause();
        }
    }

    /// <summary>
    /// Resume the game.
    /// </summary>
    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
            _simulationManager?.Resume();
        }
    }

    /// <summary>
    /// Toggle pause state.
    /// </summary>
    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
            PauseGame();
        else if (CurrentState == GameState.Paused)
            ResumeGame();
    }

    public override void _Input(InputEvent @event)
    {
        // Global input handling
        if (@event.IsActionPressed("ui_cancel"))
        {
            TogglePause();
        }
    }
}
