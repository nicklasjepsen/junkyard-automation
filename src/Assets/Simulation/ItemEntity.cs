using System;
using UnityEngine;
using JunkyardAutomation.Core;

namespace JunkyardAutomation.Simulation
{
    /// <summary>
    /// Runtime instance of an item in the yard.
    /// Holds position, movement progress, and type reference.
    /// </summary>
    public class ItemEntity
    {
        public string Id { get; private set; }
        public string TypeId { get; private set; }
        public Vector2Int Position { get; set; }
        public float Progress { get; set; }
        public int Direction { get; set; }

        /// <summary>
        /// Previous position for interpolation.
        /// </summary>
        public Vector2Int PreviousPosition { get; set; }
        public float PreviousProgress { get; set; }

        public ItemEntity(string typeId, Vector2Int position, int direction)
        {
            Id = Guid.NewGuid().ToString();
            TypeId = typeId;
            Position = position;
            PreviousPosition = position;
            Progress = 0f;
            PreviousProgress = 0f;
            Direction = direction;
        }

        /// <summary>
        /// Get world position for rendering, optionally interpolated.
        /// </summary>
        public Vector3 GetWorldPosition(float interpolation = 0f)
        {
            if (GridSystem.Instance == null)
                return Vector3.zero;

            // Get base position
            Vector3 currentWorld = GridSystem.Instance.GridToWorld(Position);

            if (interpolation <= 0f)
            {
                // Add progress offset
                Vector3 dir = GetDirectionVector();
                return currentWorld + dir * Progress;
            }

            // Interpolate between previous and current state
            Vector3 prevWorld = GridSystem.Instance.GridToWorld(PreviousPosition);
            Vector3 prevDir = GetDirectionVector();

            Vector3 prevPos = prevWorld + prevDir * PreviousProgress;
            Vector3 currPos = currentWorld + GetDirectionVector() * Progress;

            return Vector3.Lerp(prevPos, currPos, interpolation);
        }

        /// <summary>
        /// Get movement direction as world-space vector.
        /// </summary>
        public Vector3 GetDirectionVector()
        {
            if (GridSystem.Instance == null)
                return Vector3.zero;

            // Convert rotation to grid direction
            Vector2Int gridDir = GetGridDirection();

            // Convert to isometric world offset
            float tileW = GridSystem.Instance.TileWorldWidth;
            float tileH = GridSystem.Instance.TileWorldHeight;

            float worldX = (gridDir.x - gridDir.y) * tileW / 2f;
            float worldY = (gridDir.x + gridDir.y) * tileH / 2f;

            return new Vector3(worldX, worldY, 0f);
        }

        /// <summary>
        /// Get direction as grid offset (0,1 for north, 1,0 for east, etc.)
        /// </summary>
        public Vector2Int GetGridDirection()
        {
            // Direction is in degrees: 0=East, 90=North, 180=West, 270=South
            // In our grid: +X is right, +Y is up
            return Direction switch
            {
                0 => new Vector2Int(1, 0),    // East
                90 => new Vector2Int(0, 1),   // North
                180 => new Vector2Int(-1, 0), // West
                270 => new Vector2Int(0, -1), // South
                _ => new Vector2Int(1, 0)
            };
        }

        /// <summary>
        /// Get the tile this item will move to next.
        /// </summary>
        public Vector2Int GetNextTile()
        {
            return Position + GetGridDirection();
        }

        /// <summary>
        /// Save current state for interpolation.
        /// Call this at the start of each tick before updating.
        /// </summary>
        public void SavePreviousState()
        {
            PreviousPosition = Position;
            PreviousProgress = Progress;
        }
    }
}
