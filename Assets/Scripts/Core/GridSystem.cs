using UnityEngine;

namespace JunkyardAutomation.Core
{
    /// <summary>
    /// Handles isometric grid coordinate conversions and grid state.
    /// Uses 2:1 isometric ratio (tile width = 2 * tile height).
    /// </summary>
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance { get; private set; }

        [Header("Grid Configuration")]
        [SerializeField] private int gridWidth = 32;
        [SerializeField] private int gridHeight = 32;
        [SerializeField] private float tileWorldWidth = 1f;
        [SerializeField] private float tileWorldHeight = 0.5f;

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public float TileWorldWidth => tileWorldWidth;
        public float TileWorldHeight => tileWorldHeight;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Convert grid coordinates to world position (center of tile).
        /// Isometric formula:
        ///   worldX = (gridX - gridY) * (tileWidth / 2)
        ///   worldY = (gridX + gridY) * (tileHeight / 2)
        /// </summary>
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            float worldX = (gridPos.x - gridPos.y) * (tileWorldWidth / 2f);
            float worldY = (gridPos.x + gridPos.y) * (tileWorldHeight / 2f);
            return new Vector3(worldX, worldY, 0f);
        }

        /// <summary>
        /// Convert world position to grid coordinates.
        /// Inverse of GridToWorld.
        /// </summary>
        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            // Inverse isometric transformation
            float halfWidth = tileWorldWidth / 2f;
            float halfHeight = tileWorldHeight / 2f;

            // Solve for gridX and gridY:
            // worldX = (gridX - gridY) * halfWidth
            // worldY = (gridX + gridY) * halfHeight
            //
            // gridX = worldX/halfWidth/2 + worldY/halfHeight/2
            // gridY = worldY/halfHeight/2 - worldX/halfWidth/2

            float gridX = (worldPos.x / halfWidth + worldPos.y / halfHeight) / 2f;
            float gridY = (worldPos.y / halfHeight - worldPos.x / halfWidth) / 2f;

            return new Vector2Int(Mathf.FloorToInt(gridX + 0.5f), Mathf.FloorToInt(gridY + 0.5f));
        }

        /// <summary>
        /// Convert screen position to grid coordinates.
        /// </summary>
        public Vector2Int ScreenToGrid(Vector2 screenPos, Camera camera)
        {
            Vector3 worldPos = camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -camera.transform.position.z));
            return WorldToGrid(worldPos);
        }

        /// <summary>
        /// Check if grid coordinates are within bounds.
        /// </summary>
        public bool IsValidGridPosition(Vector2Int gridPos)
        {
            return gridPos.x >= 0 && gridPos.x < gridWidth &&
                   gridPos.y >= 0 && gridPos.y < gridHeight;
        }

        /// <summary>
        /// Get the world bounds of the grid (for camera clamping).
        /// </summary>
        public Bounds GetGridWorldBounds()
        {
            // Calculate corners
            Vector3 corner00 = GridToWorld(new Vector2Int(0, 0));
            Vector3 cornerMaxX = GridToWorld(new Vector2Int(gridWidth - 1, 0));
            Vector3 cornerMaxY = GridToWorld(new Vector2Int(0, gridHeight - 1));
            Vector3 cornerMaxXY = GridToWorld(new Vector2Int(gridWidth - 1, gridHeight - 1));

            float minX = Mathf.Min(corner00.x, cornerMaxX.x, cornerMaxY.x, cornerMaxXY.x) - tileWorldWidth / 2f;
            float maxX = Mathf.Max(corner00.x, cornerMaxX.x, cornerMaxY.x, cornerMaxXY.x) + tileWorldWidth / 2f;
            float minY = Mathf.Min(corner00.y, cornerMaxX.y, cornerMaxY.y, cornerMaxXY.y) - tileWorldHeight / 2f;
            float maxY = Mathf.Max(corner00.y, cornerMaxX.y, cornerMaxY.y, cornerMaxXY.y) + tileWorldHeight / 2f;

            Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);

            return new Bounds(center, size);
        }

        /// <summary>
        /// Get the four corner vertices of a tile in world space (for rendering).
        /// Order: bottom, right, top, left (clockwise from bottom)
        /// </summary>
        public Vector3[] GetTileCorners(Vector2Int gridPos)
        {
            Vector3 center = GridToWorld(gridPos);
            float halfW = tileWorldWidth / 2f;
            float halfH = tileWorldHeight / 2f;

            return new Vector3[]
            {
                center + new Vector3(0, -halfH, 0),  // bottom
                center + new Vector3(halfW, 0, 0),   // right
                center + new Vector3(0, halfH, 0),   // top
                center + new Vector3(-halfW, 0, 0)   // left
            };
        }
    }
}
