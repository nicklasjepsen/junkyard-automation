using Godot;

namespace JunkyardAutomation;

/// <summary>
/// BuildMenu - Machine selection UI for building.
/// </summary>
public partial class BuildMenu : Control
{
    [Signal]
    public delegate void MachineSelectedEventHandler(string machineTypeId);

    [Signal]
    public delegate void DemolishSelectedEventHandler();

    private VBoxContainer? _buttonContainer;
    private ContentRegistry? _contentRegistry;

    public override void _Ready()
    {
        _contentRegistry = GetNode<ContentRegistry>("/root/ContentRegistry");

        // Wait for content to load
        if (!_contentRegistry.IsLoaded)
        {
            _contentRegistry.ContentLoaded += OnContentLoaded;
        }
        else
        {
            BuildMenuButtons();
        }
    }

    private void OnContentLoaded()
    {
        BuildMenuButtons();
    }

    private void BuildMenuButtons()
    {
        // Create container if not exists
        _buttonContainer = GetNodeOrNull<VBoxContainer>("VBoxContainer");
        if (_buttonContainer == null)
        {
            _buttonContainer = new VBoxContainer();
            AddChild(_buttonContainer);
        }

        // Clear existing buttons
        foreach (var child in _buttonContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Add machine buttons
        if (_contentRegistry != null)
        {
            foreach (var machine in _contentRegistry.GetPlaceableMachines())
            {
                if (machine.TryGetValue("id", out var idVar))
                {
                    var id = idVar.AsString();
                    var displayName = id;
                    if (machine.TryGetValue("name", out var nameVar))
                    {
                        displayName = nameVar.AsString();
                    }

                    var button = new Button();
                    button.Text = displayName;
                    button.Pressed += () => OnMachineButtonPressed(id);
                    _buttonContainer.AddChild(button);
                }
            }
        }

        // Add demolish button
        var demolishButton = new Button();
        demolishButton.Text = "Demolish";
        demolishButton.Pressed += OnDemolishButtonPressed;
        _buttonContainer.AddChild(demolishButton);
    }

    private void OnMachineButtonPressed(string machineTypeId)
    {
        EmitSignal(SignalName.MachineSelected, machineTypeId);
    }

    private void OnDemolishButtonPressed()
    {
        EmitSignal(SignalName.DemolishSelected);
    }
}
