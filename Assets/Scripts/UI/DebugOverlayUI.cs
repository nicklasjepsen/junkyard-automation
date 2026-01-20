using UnityEngine;

namespace JunkyardAutomation.UI
{
    /// <summary>
    /// Debug overlay showing grid coordinates, world position, and FPS.
    /// Toggle with F3 key.
    /// </summary>
    public class DebugOverlayUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private KeyCode toggleKey = KeyCode.F3;
        [SerializeField] private bool showOnStart = true;

        [Header("Style")]
        [SerializeField] private int fontSize = 14;
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.7f);

        private bool isVisible;
        private float deltaTime;
        private GUIStyle labelStyle;
        private GUIStyle backgroundStyle;
        private Texture2D backgroundTexture;

        // Cached references
        private Camera mainCamera;
        private Core.GridSystem gridSystem;
        private Core.TileHighlighter tileHighlighter;

        private void Start()
        {
            isVisible = showOnStart;
            mainCamera = Camera.main;
            gridSystem = Core.GridSystem.Instance;
            tileHighlighter = FindObjectOfType<Core.TileHighlighter>();

            CreateStyles();
        }

        private void Update()
        {
            // Update FPS calculation
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            // Toggle visibility
            if (Input.GetKeyDown(toggleKey))
            {
                isVisible = !isVisible;
            }

            // Update cached references if needed
            if (gridSystem == null)
            {
                gridSystem = Core.GridSystem.Instance;
            }
        }

        private void CreateStyles()
        {
            // Create background texture
            backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, backgroundColor);
            backgroundTexture.Apply();

            // Label style
            labelStyle = new GUIStyle();
            labelStyle.fontSize = fontSize;
            labelStyle.normal.textColor = textColor;
            labelStyle.padding = new RectOffset(5, 5, 2, 2);

            // Background style
            backgroundStyle = new GUIStyle();
            backgroundStyle.normal.background = backgroundTexture;
        }

        private void OnGUI()
        {
            if (!isVisible) return;

            // Ensure styles exist
            if (labelStyle == null)
            {
                CreateStyles();
            }

            float padding = 10f;
            float lineHeight = fontSize + 4f;
            float boxWidth = 220f;
            float boxHeight = lineHeight * 5 + padding * 2;

            // Background box
            Rect boxRect = new Rect(padding, padding, boxWidth, boxHeight);
            GUI.Box(boxRect, GUIContent.none, backgroundStyle);

            float y = padding + 5f;

            // FPS
            float fps = 1.0f / deltaTime;
            DrawLabel(padding + 5f, y, $"FPS: {fps:F1}");
            y += lineHeight;

            // Grid coordinates
            Vector2Int? hoveredTile = tileHighlighter?.HoveredTile;
            string gridText = hoveredTile.HasValue
                ? $"Grid: ({hoveredTile.Value.x}, {hoveredTile.Value.y})"
                : "Grid: --";
            DrawLabel(padding + 5f, y, gridText);
            y += lineHeight;

            // World position
            if (mainCamera != null)
            {
                Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z)
                );
                DrawLabel(padding + 5f, y, $"World: ({mouseWorld.x:F2}, {mouseWorld.y:F2})");
            }
            else
            {
                DrawLabel(padding + 5f, y, "World: --");
            }
            y += lineHeight;

            // Camera zoom
            if (mainCamera != null)
            {
                DrawLabel(padding + 5f, y, $"Zoom: {mainCamera.orthographicSize:F1}");
            }
            y += lineHeight;

            // Grid size
            if (gridSystem != null)
            {
                DrawLabel(padding + 5f, y, $"Grid Size: {gridSystem.GridWidth}x{gridSystem.GridHeight}");
            }

            // Toggle hint at bottom
            GUI.Label(
                new Rect(padding + 5f, boxRect.yMax + 5f, boxWidth, lineHeight),
                $"Press {toggleKey} to toggle",
                labelStyle
            );
        }

        private void DrawLabel(float x, float y, string text)
        {
            GUI.Label(new Rect(x, y, 200f, fontSize + 4f), text, labelStyle);
        }

        private void OnDestroy()
        {
            if (backgroundTexture != null)
            {
                Destroy(backgroundTexture);
            }
        }
    }
}
