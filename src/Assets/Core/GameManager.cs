using UnityEngine;
using JunkyardAutomation.Data;

namespace JunkyardAutomation.Core
{
    /// <summary>
    /// Main game manager - handles initialization and game state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GridSystem gridSystem;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private GridRenderer gridRenderer;
        [SerializeField] private TileHighlighter tileHighlighter;

        public GridSystem Grid => gridSystem;
        public Vector2Int? HoveredTile => tileHighlighter != null ? tileHighlighter.HoveredTile : null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Load content definitions before anything else initializes
            LoadContent();
        }

        private void Start()
        {
            Initialize();
        }

        private void LoadContent()
        {
            ContentRegistry.Clear();
            ContentLoader.LoadAllContent();
            ContentRegistry.MarkLoaded();
        }

        private void Initialize()
        {
            // Center camera on grid
            if (cameraController != null)
            {
                cameraController.CenterOnGrid();
            }

            Debug.Log($"[GameManager] Initialized - Grid: {gridSystem.GridWidth}x{gridSystem.GridHeight}");
        }
    }
}
