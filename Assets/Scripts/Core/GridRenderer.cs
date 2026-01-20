using UnityEngine;

namespace JunkyardAutomation.Core
{
    /// <summary>
    /// Renders the isometric grid using procedural mesh generation.
    /// Creates a checkerboard pattern of diamond-shaped tiles.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class GridRenderer : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color tileColorA = new Color(0.9f, 0.9f, 0.9f, 1f);
        [SerializeField] private Color tileColorB = new Color(0.8f, 0.8f, 0.8f, 1f);
        [SerializeField] private Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private float gridLineWidth = 0.02f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh gridMesh;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            if (GridSystem.Instance == null)
            {
                Debug.LogWarning("[GridRenderer] GridSystem not found, deferring grid generation");
                return;
            }

            int width = GridSystem.Instance.GridWidth;
            int height = GridSystem.Instance.GridHeight;

            gridMesh = new Mesh();
            gridMesh.name = "IsometricGrid";

            // Each tile is a diamond (4 vertices, 2 triangles)
            int tileCount = width * height;
            Vector3[] vertices = new Vector3[tileCount * 4];
            int[] triangles = new int[tileCount * 6];
            Color[] colors = new Color[tileCount * 4];

            int vertIndex = 0;
            int triIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    Vector3[] corners = GridSystem.Instance.GetTileCorners(gridPos);

                    // Checkerboard pattern
                    Color tileColor = ((x + y) % 2 == 0) ? tileColorA : tileColorB;

                    // Add vertices (bottom, right, top, left)
                    int baseVertex = vertIndex;
                    for (int i = 0; i < 4; i++)
                    {
                        vertices[vertIndex] = corners[i];
                        colors[vertIndex] = tileColor;
                        vertIndex++;
                    }

                    // Two triangles to form diamond
                    // Triangle 1: bottom, right, top
                    triangles[triIndex++] = baseVertex + 0;
                    triangles[triIndex++] = baseVertex + 1;
                    triangles[triIndex++] = baseVertex + 2;

                    // Triangle 2: bottom, top, left
                    triangles[triIndex++] = baseVertex + 0;
                    triangles[triIndex++] = baseVertex + 2;
                    triangles[triIndex++] = baseVertex + 3;
                }
            }

            gridMesh.vertices = vertices;
            gridMesh.triangles = triangles;
            gridMesh.colors = colors;
            gridMesh.RecalculateNormals();
            gridMesh.RecalculateBounds();

            meshFilter.mesh = gridMesh;

            // Ensure we have a material that uses vertex colors
            if (meshRenderer.sharedMaterial == null)
            {
                meshRenderer.material = CreateVertexColorMaterial();
            }

            Debug.Log($"[GridRenderer] Generated {width}x{height} grid ({tileCount} tiles)");
        }

        private Material CreateVertexColorMaterial()
        {
            // Create a simple unlit material that uses vertex colors
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            Material mat = new Material(shader);
            mat.name = "GridMaterial";
            return mat;
        }

        /// <summary>
        /// Regenerate the grid (call after GridSystem changes).
        /// </summary>
        public void RefreshGrid()
        {
            if (gridMesh != null)
            {
                Destroy(gridMesh);
            }
            GenerateGrid();
        }
    }
}
