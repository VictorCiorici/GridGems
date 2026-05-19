using UnityEngine;
using System.Collections.Generic;

namespace GridGame.Config
{
    /// <summary>
    /// Holds the configuration for a predefined level.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevel", menuName = "GridGame/LevelData")]
    public class LevelData : ScriptableObject
    {
        /// <summary>
        /// The width of the grid for this level.
        /// </summary>
        public int gridWidth = 6;

        /// <summary>
        /// The height of the grid for this level.
        /// </summary>
        public int gridHeight = 6;

        [SerializeField]
        private List<GemPlacementData> gems = new List<GemPlacementData>();

        /// <summary>
        /// The list of gems placed on this level (read-only).
        /// </summary>
        public IReadOnlyList<GemPlacementData> Gems => gems;

        /// <summary>
        /// Sets the list of gems for this level.
        /// </summary>
        /// <param name="newGems">The list of gems to set.</param>
        public void SetGems(List<GemPlacementData> newGems)
        {
            gems = newGems ?? new List<GemPlacementData>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gridWidth <= 0 || gridHeight <= 0) return;

            bool[,] occupied = new bool[gridWidth, gridHeight];

            for (int i = 0; i < gems.Count; i++)
            {
                var gem = gems[i];
                if (gem == null) continue;

                if (gem.origin.x < 0 || gem.origin.x + gem.width > gridWidth ||
                    gem.origin.y < 0 || gem.origin.y + gem.height > gridHeight)
                {
                    UnityEngine.Debug.LogWarning(
                        $"LevelData '{name}': gem [{i}] ({gem.width}x{gem.height} at {gem.origin}) extends outside the grid ({gridWidth}x{gridHeight}).", this);
                    continue;
                }

                // Check for overlap
                bool overlaps = false;
                for (int dx = 0; dx < gem.width; dx++)
                {
                    for (int dy = 0; dy < gem.height; dy++)
                     {
                        int tx = gem.origin.x + dx;
                        int ty = gem.origin.y + dy;
                        if (occupied[tx, ty])
                        {
                            overlaps = true;
                        }
                        occupied[tx, ty] = true;
                    }
                }

                if (overlaps)
                {
                    UnityEngine.Debug.LogWarning(
                        $"LevelData '{name}': gem [{i}] ({gem.width}x{gem.height} at {gem.origin}) overlaps with another gem.", this);
                }
            }
        }
#endif
    }

    /// <summary>
    /// Data structure for a gem placement in a level.
    /// </summary>
    [System.Serializable]
    public class GemPlacementData
    {
        /// <summary>
        /// Width of the gem in cells.
        /// </summary>
        public int width;

        /// <summary>
        /// Height of the gem in cells.
        /// </summary>
        public int height;

        /// <summary>
        /// The bottom-left coordinate origin for the gem.
        /// </summary>
        public Vector2Int origin;
    }
}
