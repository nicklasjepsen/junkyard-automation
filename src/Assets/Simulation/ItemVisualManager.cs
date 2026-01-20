using System.Collections.Generic;
using UnityEngine;
using JunkyardAutomation.Core;
using JunkyardAutomation.Data;

namespace JunkyardAutomation.Simulation
{
    /// <summary>
    /// Manages visual representations of items on the grid.
    /// Creates, updates, and destroys item visuals with smooth interpolation.
    /// </summary>
    public class ItemVisualManager : MonoBehaviour
    {
        public static ItemVisualManager Instance { get; private set; }

        [Header("Visual Settings")]
        [SerializeField] private float itemSize = 0.3f;
        [SerializeField] private Color defaultItemColor = Color.white;

        private Dictionary<string, GameObject> itemVisuals = new Dictionary<string, GameObject>();
        private YardState yardState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Get yard state and subscribe to events
            var pm = JunkyardAutomation.Placement.PlacementManager.Instance;
            if (pm != null)
            {
                yardState = pm.YardState;
                yardState.OnItemAdded += OnItemAdded;
                yardState.OnItemRemoved += OnItemRemoved;
            }
        }

        private void Update()
        {
            if (yardState == null) return;

            // Get interpolation factor
            float interp = SimulationManager.Instance != null
                ? SimulationManager.Instance.GetTickInterpolation()
                : 0f;

            // Update all item positions
            foreach (var item in yardState.Items)
            {
                if (itemVisuals.TryGetValue(item.Id, out var visual))
                {
                    Vector3 pos = item.GetWorldPosition(interp);
                    pos.z = -0.1f; // Render above conveyors
                    visual.transform.position = pos;
                }
            }
        }

        private void OnItemAdded(ItemEntity item)
        {
            CreateVisual(item);
        }

        private void OnItemRemoved(ItemEntity item)
        {
            RemoveVisual(item.Id);
        }

        private void CreateVisual(ItemEntity item)
        {
            if (itemVisuals.ContainsKey(item.Id))
            {
                Debug.LogWarning($"[ItemVisualManager] Visual already exists for item {item.Id}");
                return;
            }

            GameObject obj = new GameObject($"Item_{item.TypeId}_{item.Id.Substring(0, 8)}");
            obj.transform.parent = transform;

            // Try to use sprite if available
            Sprite itemSprite = SpriteRegistry.Instance?.GetItemSprite(item.TypeId);

            if (itemSprite != null)
            {
                CreateSpriteVisual(obj, item, itemSprite);
            }
            else
            {
                CreateMeshVisual(obj, item);
            }

            // Set initial position
            Vector3 pos = item.GetWorldPosition();
            pos.z = -0.1f;
            obj.transform.position = pos;

            itemVisuals[item.Id] = obj;
        }

        private void CreateSpriteVisual(GameObject obj, ItemEntity item, Sprite sprite)
        {
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 10; // Render above machines

            // Scale sprite to fit item size
            float spriteSize = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
            float scale = (itemSize * 1.5f) / spriteSize;
            obj.transform.localScale = new Vector3(scale, scale, 1f);
        }

        private void CreateMeshVisual(GameObject obj, ItemEntity item)
        {
            MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();

            // Get color from registry
            Color itemColor = defaultItemColor;
            var definition = ContentRegistry.GetItem(item.TypeId);
            if (definition != null)
            {
                itemColor = definition.GetColor();
            }

            // Create diamond mesh
            Mesh mesh = CreateItemMesh(itemColor);
            meshFilter.mesh = mesh;

            // Create material
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            Material mat = new Material(shader);
            meshRenderer.material = mat;
        }

        private void RemoveVisual(string itemId)
        {
            if (itemVisuals.TryGetValue(itemId, out var visual))
            {
                Destroy(visual);
                itemVisuals.Remove(itemId);
            }
        }

        private Mesh CreateItemMesh(Color color)
        {
            Mesh mesh = new Mesh();
            mesh.name = "ItemMesh";

            float w = itemSize * 0.5f;
            float h = itemSize * 0.25f;

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(0, h, 0);    // Top
            vertices[1] = new Vector3(w, 0, 0);    // Right
            vertices[2] = new Vector3(0, -h, 0);   // Bottom
            vertices[3] = new Vector3(-w, 0, 0);   // Left

            int[] triangles = new int[6];
            triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
            triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;

            Color[] colors = new Color[4];
            for (int i = 0; i < 4; i++) colors[i] = color;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public int VisualCount => itemVisuals.Count;

        private void OnDestroy()
        {
            if (yardState != null)
            {
                yardState.OnItemAdded -= OnItemAdded;
                yardState.OnItemRemoved -= OnItemRemoved;
            }
        }
    }
}
