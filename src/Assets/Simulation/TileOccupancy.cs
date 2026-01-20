using System.Collections.Generic;
using UnityEngine;

namespace JunkyardAutomation.Simulation
{
    /// <summary>
    /// Tracks which tiles are occupied by machines.
    /// Used for placement validation and machine lookup.
    /// </summary>
    public class TileOccupancy
    {
        private Dictionary<Vector2Int, string> occupiedTiles = new Dictionary<Vector2Int, string>();

        /// <summary>
        /// Check if a specific tile is occupied.
        /// </summary>
        public bool IsOccupied(Vector2Int pos)
        {
            return occupiedTiles.ContainsKey(pos);
        }

        /// <summary>
        /// Check if a machine with given size can be placed at position.
        /// </summary>
        public bool CanPlace(Vector2Int pos, Vector2Int size)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int checkPos = new Vector2Int(pos.x + x, pos.y + y);
                    if (IsOccupied(checkPos))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Mark a tile as occupied by a machine.
        /// </summary>
        public void Occupy(Vector2Int pos, string machineId)
        {
            occupiedTiles[pos] = machineId;
        }

        /// <summary>
        /// Mark multiple tiles as occupied (for multi-tile machines).
        /// </summary>
        public void OccupyArea(Vector2Int pos, Vector2Int size, string machineId)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Occupy(new Vector2Int(pos.x + x, pos.y + y), machineId);
                }
            }
        }

        /// <summary>
        /// Clear a tile (when demolishing).
        /// </summary>
        public void Clear(Vector2Int pos)
        {
            occupiedTiles.Remove(pos);
        }

        /// <summary>
        /// Clear multiple tiles.
        /// </summary>
        public void ClearArea(Vector2Int pos, Vector2Int size)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Clear(new Vector2Int(pos.x + x, pos.y + y));
                }
            }
        }

        /// <summary>
        /// Get the machine ID occupying a tile, or null if empty.
        /// </summary>
        public string GetMachineAt(Vector2Int pos)
        {
            return occupiedTiles.TryGetValue(pos, out string machineId) ? machineId : null;
        }

        /// <summary>
        /// Get total number of occupied tiles.
        /// </summary>
        public int OccupiedCount => occupiedTiles.Count;
    }
}
