using UnityEngine;
using JunkyardAutomation.Core;

namespace JunkyardAutomation.Placement
{
    /// <summary>
    /// Visual preview for machine placement.
    /// Shows a semi-transparent ghost that follows the mouse.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PlacementGhost : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color validColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
        [SerializeField] private Color invalidColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private Color demolishColor = new Color(0.8f, 0.4f, 0.1f, 0.5f);

        [Header("Arrow")]
        [SerializeField] private float arrowSize = 0.3f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh ghostMesh;

        private Vector2Int currentPosition;
        private int currentRotation;
        private bool isValid;
        private bool isVisible;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            CreateMesh();
            Hide();
        }

        private void CreateMesh()
        {
            ghostMesh = new Mesh();
            ghostMesh.name = "PlacementGhost";

            // Diamond shape (4 vertices) + arrow (3 vertices)
            Vector3[] vertices = new Vector3[7];
            int[] triangles = new int[9]; // 2 triangles for diamond + 1 for arrow
            Color[] colors = new Color[7];

            // Initialize with default values (will be updated in UpdateMesh)
            for (int i = 0; i < 7; i++)
            {
                vertices[i] = Vector3.zero;
                colors[i] = validColor;
            }

            // Diamond triangles
            triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
            triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;

            // Arrow triangle
            triangles[6] = 4; triangles[7] = 5; triangles[8] = 6;

            ghostMesh.vertices = vertices;
            ghostMesh.triangles = triangles;
            ghostMesh.colors = colors;

            meshFilter.mesh = ghostMesh;

            // Create material
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Unlit/Transparent");

            Material mat = new Material(shader);
            mat.name = "GhostMaterial";
            meshRenderer.material = mat;
        }

        /// <summary>
        /// Show the ghost at specified position and rotation.
        /// </summary>
        public void Show(Vector2Int gridPos, int rotation, bool valid)
        {
            currentPosition = gridPos;
            currentRotation = rotation;
            isValid = valid;
            isVisible = true;

            UpdateMesh();
            meshRenderer.enabled = true;
        }

        /// <summary>
        /// Show demolish indicator at position.
        /// </summary>
        public void ShowDemolish(Vector2Int gridPos)
        {
            currentPosition = gridPos;
            currentRotation = 0;
            isValid = true; // Use demolish color instead
            isVisible = true;

            UpdateMeshDemolish();
            meshRenderer.enabled = true;
        }

        /// <summary>
        /// Hide the ghost.
        /// </summary>
        public void Hide()
        {
            isVisible = false;
            meshRenderer.enabled = false;
        }

        private void UpdateMesh()
        {
            if (GridSystem.Instance == null) return;

            Vector3[] corners = GridSystem.Instance.GetTileCorners(currentPosition);
            Vector3 center = GridSystem.Instance.GridToWorld(currentPosition);

            // Tile vertices
            Vector3[] vertices = ghostMesh.vertices;
            float zOffset = -0.02f; // Render above grid

            // Diamond
            vertices[0] = corners[0] + new Vector3(0, 0, zOffset); // bottom
            vertices[1] = corners[1] + new Vector3(0, 0, zOffset); // right
            vertices[2] = corners[2] + new Vector3(0, 0, zOffset); // top
            vertices[3] = corners[3] + new Vector3(0, 0, zOffset); // left

            // Arrow pointing in direction
            Vector3 arrowDir = GetArrowDirection();
            Vector3 arrowBase = center + new Vector3(0, 0, zOffset);
            Vector3 arrowTip = arrowBase + arrowDir * arrowSize;
            Vector3 arrowPerp = new Vector3(-arrowDir.y, arrowDir.x, 0) * arrowSize * 0.4f;

            vertices[4] = arrowBase - arrowPerp;
            vertices[5] = arrowTip;
            vertices[6] = arrowBase + arrowPerp;

            ghostMesh.vertices = vertices;

            // Update colors
            Color color = isValid ? validColor : invalidColor;
            Color[] colors = ghostMesh.colors;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
            ghostMesh.colors = colors;

            ghostMesh.RecalculateBounds();
        }

        private void UpdateMeshDemolish()
        {
            if (GridSystem.Instance == null) return;

            Vector3[] corners = GridSystem.Instance.GetTileCorners(currentPosition);

            Vector3[] vertices = ghostMesh.vertices;
            float zOffset = -0.02f;

            // Diamond only, no arrow for demolish
            vertices[0] = corners[0] + new Vector3(0, 0, zOffset);
            vertices[1] = corners[1] + new Vector3(0, 0, zOffset);
            vertices[2] = corners[2] + new Vector3(0, 0, zOffset);
            vertices[3] = corners[3] + new Vector3(0, 0, zOffset);

            // Collapse arrow vertices to center
            Vector3 center = GridSystem.Instance.GridToWorld(currentPosition) + new Vector3(0, 0, zOffset);
            vertices[4] = center;
            vertices[5] = center;
            vertices[6] = center;

            ghostMesh.vertices = vertices;

            // Demolish color
            Color[] colors = ghostMesh.colors;
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = demolishColor;
            }
            ghostMesh.colors = colors;

            ghostMesh.RecalculateBounds();
        }

        private Vector3 GetArrowDirection()
        {
            // Convert rotation to world direction
            // In isometric: 0° = +X (right), 90° = +Y (up-right), etc.
            float rad = currentRotation * Mathf.Deg2Rad;

            // Adjust for isometric projection
            float x = Mathf.Cos(rad);
            float y = Mathf.Sin(rad);

            // Convert grid direction to isometric world direction
            float tileW = GridSystem.Instance.TileWorldWidth / 2f;
            float tileH = GridSystem.Instance.TileWorldHeight / 2f;

            float worldX = (x - y) * tileW;
            float worldY = (x + y) * tileH;

            return new Vector3(worldX, worldY, 0).normalized;
        }
    }
}
