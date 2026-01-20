using Godot;

namespace JunkyardAutomation;

/// <summary>
/// MachineVisual - Visual representation of a placed machine.
/// </summary>
public partial class MachineVisual : Node2D
{
    private PlacedMachine? _machine;
    private Sprite2D? _sprite;
    private GridSystem? _gridSystem;

    /// <summary>
    /// Initialize the visual for a machine.
    /// </summary>
    public void Initialize(PlacedMachine machine, GridSystem gridSystem)
    {
        _machine = machine;
        _gridSystem = gridSystem;

        // Create sprite
        _sprite = new Sprite2D();
        AddChild(_sprite);

        // Set up appearance based on machine type
        UpdateAppearance();

        // Position on grid
        Position = gridSystem.GridToWorld(machine.GridPosition);
        _sprite.RotationDegrees = machine.Rotation;
    }

    /// <summary>
    /// Update the visual appearance.
    /// </summary>
    public void UpdateAppearance()
    {
        if (_sprite == null || _machine == null) return;

        // Create placeholder texture based on machine type
        var texture = CreatePlaceholderTexture(_machine.TypeId);
        _sprite.Texture = texture;
    }

    /// <summary>
    /// Update visual state (for animations, state indicators, etc.)
    /// </summary>
    public void UpdateState()
    {
        if (_machine == null || _sprite == null) return;

        // Tint based on state
        _sprite.Modulate = _machine.State switch
        {
            MachineState.Running => Colors.White,
            MachineState.Blocked => new Color(1, 0.5f, 0.5f),
            MachineState.Stalled => new Color(1, 1, 0.5f),
            _ => Colors.White
        };
    }

    private static ImageTexture CreatePlaceholderTexture(string machineType)
    {
        // Create a simple diamond shape with type-specific color
        var image = Image.CreateEmpty(64, 32, false, Image.Format.Rgba8);
        image.Fill(Colors.Transparent);

        // Choose color based on machine type
        var color = machineType switch
        {
            "conveyor" => new Color(0.4f, 0.4f, 0.8f),    // Blue
            "splitter" => new Color(0.8f, 0.4f, 0.8f),    // Purple
            "shredder" => new Color(0.8f, 0.4f, 0.4f),    // Red
            "smelter" => new Color(0.8f, 0.6f, 0.2f),     // Orange
            _ => new Color(0.6f, 0.6f, 0.6f)              // Gray
        };

        // Draw diamond shape
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float dx = Mathf.Abs(x - 32) / 32.0f;
                float dy = Mathf.Abs(y - 16) / 16.0f;
                if (dx + dy <= 1.0f)
                {
                    // Edge detection for outline
                    if (dx + dy > 0.85f)
                    {
                        image.SetPixel(x, y, Colors.White);
                    }
                    else
                    {
                        image.SetPixel(x, y, color);
                    }
                }
            }
        }

        // Draw direction indicator (arrow)
        for (int x = 32; x < 48; x++)
        {
            if (x < 64)
            {
                image.SetPixel(x, 15, Colors.White);
                image.SetPixel(x, 16, Colors.White);
            }
        }

        var texture = ImageTexture.CreateFromImage(image);
        return texture;
    }
}
