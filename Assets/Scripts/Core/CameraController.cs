using UnityEngine;

namespace JunkyardAutomation.Core
{
    /// <summary>
    /// Controls camera pan and zoom for the isometric view.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 15f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float zoomSmoothTime = 0.1f;

        [Header("Pan Settings")]
        [SerializeField] private float keyboardPanSpeed = 10f;
        [SerializeField] private float dragPanSpeed = 1f;
        [SerializeField] private bool clampToBounds = true;

        private Camera cam;
        private float targetZoom;
        private float zoomVelocity;
        private Vector3 lastMousePosition;
        private bool isDragging;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            targetZoom = cam.orthographicSize;
        }

        private void Update()
        {
            HandleZoom();
            HandlePan();
            ClampCameraPosition();
        }

        private void HandleZoom()
        {
            float scrollDelta = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                targetZoom -= scrollDelta * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }

            // Smooth zoom
            cam.orthographicSize = Mathf.SmoothDamp(
                cam.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime
            );
        }

        private void HandlePan()
        {
            Vector3 panDelta = Vector3.zero;

            // Keyboard pan (WASD and arrows)
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
            {
                panDelta = new Vector3(horizontal, vertical, 0f).normalized * keyboardPanSpeed * Time.deltaTime;
            }

            // Mouse drag pan (middle mouse button)
            if (Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
                // Convert screen delta to world delta (invert for natural drag feel)
                float worldDeltaX = -mouseDelta.x * cam.orthographicSize * 2f / Screen.height * dragPanSpeed;
                float worldDeltaY = -mouseDelta.y * cam.orthographicSize * 2f / Screen.height * dragPanSpeed;
                panDelta += new Vector3(worldDeltaX, worldDeltaY, 0f);
                lastMousePosition = Input.mousePosition;
            }

            // Apply pan
            transform.position += panDelta;
        }

        private void ClampCameraPosition()
        {
            if (!clampToBounds || GridSystem.Instance == null) return;

            Bounds gridBounds = GridSystem.Instance.GetGridWorldBounds();

            // Calculate visible area
            float verticalSize = cam.orthographicSize;
            float horizontalSize = verticalSize * cam.aspect;

            // Clamp camera position to keep grid in view
            float minX = gridBounds.min.x + horizontalSize;
            float maxX = gridBounds.max.x - horizontalSize;
            float minY = gridBounds.min.y + verticalSize;
            float maxY = gridBounds.max.y - verticalSize;

            // If grid is smaller than view, center it
            if (minX > maxX)
            {
                minX = maxX = gridBounds.center.x;
            }
            if (minY > maxY)
            {
                minY = maxY = gridBounds.center.y;
            }

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }

        /// <summary>
        /// Center camera on a specific grid position.
        /// </summary>
        public void CenterOn(Vector2Int gridPos)
        {
            if (GridSystem.Instance == null) return;
            Vector3 worldPos = GridSystem.Instance.GridToWorld(gridPos);
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        }

        /// <summary>
        /// Center camera on the middle of the grid.
        /// </summary>
        public void CenterOnGrid()
        {
            if (GridSystem.Instance == null) return;
            Vector2Int center = new Vector2Int(
                GridSystem.Instance.GridWidth / 2,
                GridSystem.Instance.GridHeight / 2
            );
            CenterOn(center);
        }
    }
}
