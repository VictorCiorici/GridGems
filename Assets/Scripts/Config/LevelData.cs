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

        /// <summary>
        /// The list of gems placed on this level.
        /// </summary>
        public List<GemPlacementData> gems = new List<GemPlacementData>();
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
