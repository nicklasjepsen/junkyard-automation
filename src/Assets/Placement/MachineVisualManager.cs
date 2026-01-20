using System.Collections.Generic;
using UnityEngine;
using JunkyardAutomation.Core;
using JunkyardAutomation.Data;
using JunkyardAutomation.Simulation;

namespace JunkyardAutomation.Placement
{
    /// <summary>
    /// Manages visual representations of placed machines.
    /// Creates and destroys GameObjects for machines in the yard.
    /// </summary>
    public class MachineVisualManager : MonoBehaviour
    {
        public static MachineVisualManager Instance { get; private set; }

        [Header("Visual Settings")]
        [SerializeField] private Color defaultMachineColor = new Color(0.4f, 0.4f, 0.5f, 1f);
        [SerializeField] private Color arrowColor = new Color(0.9f, 0.9f, 0.3f, 1f);
        [SerializeField] private float arrowSize = 0.25f;

        private Dictionary<string, GameObject> machineVisuals = new Dictionary<string, GameObject>();

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
        /// Create visual representation for a placed machine.
        /// </summary>
        public void CreateVisual(PlacedMachine machine)
        {
            if (machineVisuals.ContainsKey(machine.Id))
            {
                Debug.LogWarning($"[MachineVisualManager] Visual already exists for {machine.Id}");
                return;
            }

            GameObject visual = CreateConveyorVisual(machine);
            machineVisuals[machine.Id] = visual;
        }

        /// <summary>
        /// Remove visual representation for a machine.
        /// </summary>
        public void RemoveVisual(string machineId)
        {
            if (machineVisuals.TryGetValue(machineId, out var visual))
            {
                Destroy(visual);
                machineVisuals.Remove(machineId);
            }
        }

        private GameObject CreateConveyorVisual(PlacedMachine machine)
        {
            GameObject obj = new GameObject($"Machine_{machine.MachineTypeId}_{machine.Id.Substring(0, 8)}");
            obj.transform.parent = transform;

            // Try to use sprite if available
            Sprite machineSprite = SpriteRegistry.Instance?.GetMachineSprite(machine.MachineTypeId);

            if (machineSprite != null)
            {
                // Use sprite renderer
                CreateSpriteVisual(obj, machine, machineSprite);
            }
            else
            {
                // Fall back to procedural mesh
                CreateMeshVisual(obj, machine);
            }

            return obj;
        }

        private void CreateSpriteVisual(GameObject obj, PlacedMachine machine, Sprite sprite)
        {
            Vector3 worldPos = GridSystem.Instance.GridToWorld(machine.Position);
            worldPos.z = -0.005f; // Render above grid

            obj.transform.position = worldPos;

            // For conveyors, rotate the sprite based on direction
            if (machine.MachineTypeId == "Conveyor")
            {
                obj.transform.rotation = Quaternion.Euler(0, 0, machine.Rotation - 90); // Adjust for sprite orientation
            }

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 1;

            // Scale sprite to fit tile
            float tileSize = GridSystem.Instance.TileWorldWidth;
            float spriteSize = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
            float scale = (tileSize * 0.8f) / spriteSize; // 80% of tile size
            obj.transform.localScale = new Vector3(scale, scale, 1f);
        }

        private void CreateMeshVisual(GameObject obj, PlacedMachine machine)
        {
            // Add mesh components
            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();

            // Get color from registry
            Color machineColor = defaultMachineColor;
            var definition = ContentRegistry.GetMachine(machine.MachineTypeId);
            if (definition != null)
            {
                machineColor = definition.GetColor();
            }

            // Create mesh
            Mesh mesh = CreateConveyorMesh(machine.Position, machine.Rotation, machineColor);
            meshFilter.mesh = mesh;

            // Create material
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            Material mat = new Material(shader);
            meshRenderer.material = mat;
        }

        private Mesh CreateConveyorMesh(Vector2Int position, int rotation, Color machineColor)
        {
            if (GridSystem.Instance == null) return new Mesh();

            Mesh mesh = new Mesh();
            mesh.name = "ConveyorMesh";

            Vector3[] corners = GridSystem.Instance.GetTileCorners(position);
            Vector3 center = GridSystem.Instance.GridToWorld(position);

            float zOffset = -0.005f; // Render above grid but below ghost

            // 4 vertices for diamond + 3 for arrow = 7 vertices
            Vector3[] vertices = new Vector3[7];
            int[] triangles = new int[9];
            Color[] colors = new Color[7];

            // Diamond
            vertices[0] = corners[0] + new Vector3(0, 0, zOffset);
            vertices[1] = corners[1] + new Vector3(0, 0, zOffset);
            vertices[2] = corners[2] + new Vector3(0, 0, zOffset);
            vertices[3] = corners[3] + new Vector3(0, 0, zOffset);

            // Arrow
            Vector3 arrowDir = GetArrowDirection(rotation);
            Vector3 arrowBase = center + new Vector3(0, 0, zOffset - 0.001f);
            Vector3 arrowTip = arrowBase + arrowDir * arrowSize;
            Vector3 arrowPerp = new Vector3(-arrowDir.y, arrowDir.x, 0) * arrowSize * 0.4f;

            vertices[4] = arrowBase - arrowPerp;
            vertices[5] = arrowTip;
            vertices[6] = arrowBase + arrowPerp;

            // Diamond triangles
            triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
            triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;

            // Arrow triangle
            triangles[6] = 4; triangles[7] = 5; triangles[8] = 6;

            // Colors
            for (int i = 0; i < 4; i++) colors[i] = machineColor;
            for (int i = 4; i < 7; i++) colors[i] = arrowColor;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private Vector3 GetArrowDirection(int rotation)
        {
            float rad = rotation * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad);
            float y = Mathf.Sin(rad);

            float tileW = GridSystem.Instance.TileWorldWidth / 2f;
            float tileH = GridSystem.Instance.TileWorldHeight / 2f;

            float worldX = (x - y) * tileW;
            float worldY = (x + y) * tileH;

            return new Vector3(worldX, worldY, 0).normalized;
        }

        /// <summary>
        /// Get count of active visuals.
        /// </summary>
        public int VisualCount => machineVisuals.Count;
    }
}
