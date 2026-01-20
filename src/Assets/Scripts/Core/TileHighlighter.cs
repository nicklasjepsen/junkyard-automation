using UnityEngine;

namespace JunkyardAutomation.Core
{
    /// <summary>
    /// Highlights the tile under the mouse cursor.
    /// Creates a diamond-shaped highlight overlay.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TileHighlighter : MonoBehaviour
    {
        [Header("Highlight Settings")]
        [SerializeField] private Color highlightColor = new Color(0.2f, 0.6f, 1f, 0.4f);
        [SerializeField] private Color invalidColor = new Color(1f, 0.2f, 0.2f, 0.4f);

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh highlightMesh;
        private Camera mainCamera;

        private Vector2Int? hoveredTile;
        private bool isValid = true;

        public Vector2Int? HoveredTile => hoveredTile;
        public bool IsValidPosition => isValid;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            CreateHighlightMesh();
        }

        private void Start()
        {
            mainCamera = Camera.main;

            // Start hidden
            meshRenderer.enabled = false;
        }

        private void Update()
        {
            UpdateHoveredTile();
        }

        private void CreateHighlightMesh()
        {
            highlightMesh = new Mesh();
            highlightMesh.name = "TileHighlight";

            // Diamond shape (4 vertices)
            Vector3[] vertices = new Vector3[4];
            int[] triangles = new int[6];
            Color[] colors = new Color[4];

            // Will be updated with actual positions in UpdateHighlightPosition
            for (int i = 0; i < 4; i++)
            {
                vertices[i] = Vector3.zero;
                colors[i] = highlightColor;
            }

            // Two triangles
            triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
            triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;

            highlightMesh.vertices = vertices;
            highlightMesh.triangles = triangles;
            highlightMesh.colors = colors;

            meshFilter.mesh = highlightMesh;

            // Create material
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Transparent");
            }

            Material mat = new Material(shader);
            mat.name = "HighlightMaterial";
            mat.color = Color.white; // Use vertex colors
            meshRenderer.material = mat;
        }

        private void UpdateHoveredTile()
        {
            if (mainCamera == null || GridSystem.Instance == null)
            {
                hoveredTile = null;
                meshRenderer.enabled = false;
                return;
            }

            Vector2 mousePos = Input.mousePosition;
            Vector2Int gridPos = GridSystem.Instance.ScreenToGrid(mousePos, mainCamera);

            // Check if position is valid
            isValid = GridSystem.Instance.IsValidGridPosition(gridPos);

            if (isValid)
            {
                hoveredTile = gridPos;
                UpdateHighlightPosition(gridPos);
                meshRenderer.enabled = true;
            }
            else
            {
                hoveredTile = null;
                meshRenderer.enabled = false;
            }
        }

        private void UpdateHighlightPosition(Vector2Int gridPos)
        {
            if (GridSystem.Instance == null) return;

            Vector3[] corners = GridSystem.Instance.GetTileCorners(gridPos);

            // Update mesh vertices
            Vector3[] vertices = highlightMesh.vertices;
            for (int i = 0; i < 4; i++)
            {
                // Offset slightly in Z to render above grid
                vertices[i] = corners[i] + new Vector3(0, 0, -0.01f);
            }
            highlightMesh.vertices = vertices;
            highlightMesh.RecalculateBounds();
        }

        /// <summary>
        /// Set the highlight color (for different build modes).
        /// </summary>
        public void SetHighlightColor(Color color)
        {
            highlightColor = color;
            UpdateMeshColors();
        }

        /// <summary>
        /// Set whether current position is valid (changes color).
        /// </summary>
        public void SetValid(bool valid)
        {
            isValid = valid;
            UpdateMeshColors();
        }

        private void UpdateMeshColors()
        {
            if (highlightMesh == null) return;

            Color[] colors = highlightMesh.colors;
            Color targetColor = isValid ? highlightColor : invalidColor;

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = targetColor;
            }

            highlightMesh.colors = colors;
        }
    }
}
