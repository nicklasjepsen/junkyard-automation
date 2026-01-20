using Godot;

namespace JunkyardAutomation;

/// <summary>
/// PlacementGhost - Preview sprite for machine placement.
/// </summary>
public partial class PlacementGhost : Node2D
{
    /// <summary>Valid placement color tint</summary>
    [Export]
    public Color ValidColor { get; set; } = new Color(0, 1, 0, 0.5f);

    /// <summary>Invalid placement color tint</summary>
    [Export]
    public Color InvalidColor { get; set; } = new Color(1, 0, 0, 0.5f);

    private Sprite2D? _sprite;
    private string _currentMachineType = "";
    private int _currentRotation = 0;
    private GridSystem? _gridSystem;

    public override void _Ready()
    {
        _gridSystem = GetNode<GridSystem>("/root/GridSystem");

        // Create sprite child
        _sprite = new Sprite2D();
        AddChild(_sprite);

        // Start hidden
        Hide();
    }

    /// <summary>
    /// Set the machine type to preview.
    /// </summary>
    public void SetMachineType(string machineTypeId)
    {
        _currentMachineType = machineTypeId;
        UpdateSprite();
    }

    /// <summary>
    /// Set the rotation for the preview.
    /// </summary>
    public void SetRotation(int rotation)
    {
        _currentRotation = rotation;
        if (_sprite != null)
        {
            _sprite.RotationDegrees = rotation;
        }
    }

    /// <summary>
    /// Update position to match a grid position.
    /// </summary>
    public void UpdatePosition(Vector2I gridPos)
    {
        if (_gridSystem != null)
        {
            Position = _gridSystem.GridToWorld(gridPos);
        }
    }

    /// <summary>
    /// Set to valid placement appearance.
    /// </summary>
    public void SetValid()
    {
        if (_sprite != null)
        {
            _sprite.Modulate = ValidColor;
        }
    }

    /// <summary>
    /// Set to invalid placement appearance.
    /// </summary>
    public void SetInvalid()
    {
        if (_sprite != null)
        {
            _sprite.Modulate = InvalidColor;
        }
    }

    private void UpdateSprite()
    {
        if (_sprite == null) return;

        // For now, create a placeholder sprite
        // In the future, this would load from the content registry
        var texture = CreatePlaceholderTexture();
        _sprite.Texture = texture;
    }

    private static ImageTexture CreatePlaceholderTexture()
    {
        // Create a simple diamond shape as placeholder
        var image = Image.CreateEmpty(64, 32, false, Image.Format.Rgba8);
        image.Fill(Colors.Transparent);

        // Draw diamond shape
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                // Diamond formula
                float dx = Mathf.Abs(x - 32) / 32.0f;
                float dy = Mathf.Abs(y - 16) / 16.0f;
                if (dx + dy <= 1.0f)
                {
                    image.SetPixel(x, y, Colors.White);
                }
            }
        }

        var texture = ImageTexture.CreateFromImage(image);
        return texture;
    }
}
