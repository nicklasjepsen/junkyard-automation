using UnityEngine;

namespace JunkyardAutomation.Simulation
{
    /// <summary>
    /// Represents a machine that has been placed in the yard.
    /// </summary>
    [System.Serializable]
    public class PlacedMachine
    {
        public string Id;               // Unique instance ID (GUID)
        public string MachineTypeId;    // Reference to machine definition ("Conveyor", etc.)
        public Vector2Int Position;     // Grid position (bottom-left for multi-tile)
        public int Rotation;            // 0, 90, 180, 270 degrees

        /// <summary>
        /// Get the direction vector based on rotation.
        /// 0° = Right (+X), 90° = Up (+Y), 180° = Left (-X), 270° = Down (-Y)
        /// In isometric this maps to the visual direction.
        /// </summary>
        public Vector2Int GetDirection()
        {
            return Rotation switch
            {
                0 => new Vector2Int(1, 0),    // Right
                90 => new Vector2Int(0, 1),   // Up
                180 => new Vector2Int(-1, 0), // Left
                270 => new Vector2Int(0, -1), // Down
                _ => new Vector2Int(1, 0)
            };
        }

        /// <summary>
        /// Get the output tile position (where items exit).
        /// For a 1x1 conveyor, this is the adjacent tile in the direction.
        /// </summary>
        public Vector2Int GetOutputTile()
        {
            return Position + GetDirection();
        }

        /// <summary>
        /// Get the input tile position (where items enter).
        /// For a 1x1 conveyor, this is the adjacent tile opposite to direction.
        /// </summary>
        public Vector2Int GetInputTile()
        {
            return Position - GetDirection();
        }
    }
}
