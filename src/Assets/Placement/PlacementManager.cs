using UnityEngine;
using UnityEngine.InputSystem;
using JunkyardAutomation.Core;
using JunkyardAutomation.Data;
using JunkyardAutomation.Simulation;

namespace JunkyardAutomation.Placement
{
    public enum PlacementMode
    {
        None,
        Build,
        Demolish
    }

    /// <summary>
    /// Manages machine placement and demolition.
    /// Handles input, ghost preview, and placement validation.
    /// </summary>
    public class PlacementManager : MonoBehaviour
    {
        public static PlacementManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PlacementGhost ghost;

        [Header("Settings")]
        [SerializeField] private Vector2Int defaultMachineSize = new Vector2Int(1, 1);

        // State
        private PlacementMode currentMode = PlacementMode.None;
        private string selectedMachineType = null;
        private int currentRotation = 0;
        private YardState yardState;

        // Events
        public event System.Action<PlacementMode> OnModeChanged;
        public event System.Action<string> OnMachineTypeSelected;
        public event System.Action<int> OnRotationChanged;

        // Properties
        public PlacementMode CurrentMode => currentMode;
        public string SelectedMachineType => selectedMachineType;
        public int CurrentRotation => currentRotation;
        public YardState YardState => yardState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            yardState = new YardState();
        }

        private void Start()
        {
            // Subscribe to yard state events to update visuals
            yardState.OnMachinePlaced += OnMachinePlacedHandler;
            yardState.OnMachineRemoved += OnMachineRemovedHandler;
        }

        private void Update()
        {
            HandleInput();
            UpdateGhost();
        }

        private void HandleInput()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (keyboard == null || mouse == null) return;

            // Rotate (R key)
            if (keyboard.rKey.wasPressedThisFrame && currentMode == PlacementMode.Build)
            {
                Rotate();
            }

            // Cancel (ESC or Right-click)
            if (keyboard.escapeKey.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)
            {
                CancelPlacement();
            }

            // Place or Demolish (Left-click)
            if (mouse.leftButton.wasPressedThisFrame)
            {
                HandleClick();
            }

            // Debug: Spawn item (SPACE key)
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                TrySpawnItem();
            }
        }

        private void TrySpawnItem()
        {
            var hoveredTile = GetHoveredTile();
            if (!hoveredTile.HasValue) return;

            // Check if there's a conveyor at this position
            var machine = yardState.GetMachineAt(hoveredTile.Value);
            if (machine == null || machine.MachineTypeId != "Conveyor")
            {
                Debug.Log("[PlacementManager] Can only spawn items on conveyors");
                return;
            }

            // Spawn a ScrapFerrous item
            yardState.AddItem("ScrapFerrous", hoveredTile.Value, machine.Rotation);
        }

        private void HandleClick()
        {
            if (currentMode == PlacementMode.None) return;

            var hoveredTile = GetHoveredTile();
            if (!hoveredTile.HasValue) return;

            if (currentMode == PlacementMode.Build)
            {
                TryPlace(hoveredTile.Value);
            }
            else if (currentMode == PlacementMode.Demolish)
            {
                TryDemolish(hoveredTile.Value);
            }
        }

        private void TryPlace(Vector2Int position)
        {
            if (string.IsNullOrEmpty(selectedMachineType)) return;

            // Check grid bounds
            if (!GridSystem.Instance.IsValidGridPosition(position)) return;

            // Get machine size from registry
            Vector2Int size = GetMachineSize(selectedMachineType);

            // Try to place
            var machine = yardState.PlaceMachine(
                selectedMachineType,
                position,
                currentRotation,
                size
            );

            if (machine != null)
            {
                // Successfully placed - could play sound, show effect, etc.
            }
        }

        private void TryDemolish(Vector2Int position)
        {
            var machine = yardState.GetMachineAt(position);
            if (machine == null) return;

            Vector2Int size = GetMachineSize(machine.MachineTypeId);
            yardState.RemoveMachine(machine.Id, size);
        }

        /// <summary>
        /// Get machine size from registry, or default if not found.
        /// </summary>
        private Vector2Int GetMachineSize(string machineType)
        {
            var definition = ContentRegistry.GetMachine(machineType);
            if (definition != null)
            {
                return definition.GetSize();
            }
            return defaultMachineSize;
        }

        private void UpdateGhost()
        {
            if (ghost == null) return;

            if (currentMode == PlacementMode.Build && !string.IsNullOrEmpty(selectedMachineType))
            {
                var hoveredTile = GetHoveredTile();
                if (hoveredTile.HasValue && GridSystem.Instance.IsValidGridPosition(hoveredTile.Value))
                {
                    Vector2Int size = GetMachineSize(selectedMachineType);
                    bool canPlace = yardState.CanPlaceAt(hoveredTile.Value, size);
                    ghost.Show(hoveredTile.Value, currentRotation, canPlace);
                }
                else
                {
                    ghost.Hide();
                }
            }
            else if (currentMode == PlacementMode.Demolish)
            {
                var hoveredTile = GetHoveredTile();
                if (hoveredTile.HasValue)
                {
                    var machine = yardState.GetMachineAt(hoveredTile.Value);
                    if (machine != null)
                    {
                        ghost.ShowDemolish(hoveredTile.Value);
                    }
                    else
                    {
                        ghost.Hide();
                    }
                }
                else
                {
                    ghost.Hide();
                }
            }
            else
            {
                ghost.Hide();
            }
        }

        private Vector2Int? GetHoveredTile()
        {
            if (GridSystem.Instance == null) return null;

            var mouse = Mouse.current;
            if (mouse == null) return null;

            Vector2 mousePos = mouse.position.ReadValue();
            Vector2Int gridPos = GridSystem.Instance.ScreenToGrid(mousePos, Camera.main);

            if (GridSystem.Instance.IsValidGridPosition(gridPos))
            {
                return gridPos;
            }

            return null;
        }

        /// <summary>
        /// Enter build mode with specified machine type.
        /// </summary>
        public void EnterBuildMode(string machineType)
        {
            selectedMachineType = machineType;
            currentMode = PlacementMode.Build;
            currentRotation = 0;

            Debug.Log($"[PlacementManager] Entered build mode: {machineType}");

            OnModeChanged?.Invoke(currentMode);
            OnMachineTypeSelected?.Invoke(machineType);
        }

        /// <summary>
        /// Enter demolish mode.
        /// </summary>
        public void EnterDemolishMode()
        {
            selectedMachineType = null;
            currentMode = PlacementMode.Demolish;

            Debug.Log("[PlacementManager] Entered demolish mode");

            OnModeChanged?.Invoke(currentMode);
        }

        /// <summary>
        /// Exit any placement mode.
        /// </summary>
        public void CancelPlacement()
        {
            if (currentMode == PlacementMode.None) return;

            currentMode = PlacementMode.None;
            selectedMachineType = null;

            Debug.Log("[PlacementManager] Exited placement mode");

            OnModeChanged?.Invoke(currentMode);
        }

        /// <summary>
        /// Rotate the ghost 90° clockwise.
        /// </summary>
        public void Rotate()
        {
            currentRotation = (currentRotation + 90) % 360;

            Debug.Log($"[PlacementManager] Rotation: {currentRotation}°");

            OnRotationChanged?.Invoke(currentRotation);
        }

        private void OnMachinePlacedHandler(PlacedMachine machine)
        {
            // Create visual representation
            MachineVisualManager.Instance?.CreateVisual(machine);
        }

        private void OnMachineRemovedHandler(PlacedMachine machine)
        {
            // Remove visual representation
            MachineVisualManager.Instance?.RemoveVisual(machine.Id);
        }

        private void OnDestroy()
        {
            if (yardState != null)
            {
                yardState.OnMachinePlaced -= OnMachinePlacedHandler;
                yardState.OnMachineRemoved -= OnMachineRemovedHandler;
            }
        }
    }
}
