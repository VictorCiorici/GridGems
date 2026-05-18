using UnityEngine;

namespace GridGame.Config
{
    /// <summary>
    /// Configuration for a specific gem type, mapping size to sprite.
    /// </summary>
    [CreateAssetMenu(fileName = "NewGemVisualData", menuName = "GridGame/GemVisualData")]
    public class GemVisualData : ScriptableObject
    {
        /// <summary>
        /// Height of the gem in cells.
        /// </summary>
        public int height;

        /// <summary>
        /// Width of the gem in cells.
        /// </summary>
        public int width;

        /// <summary>
        /// The sprite to display for this gem.
        /// </summary>
        public Sprite sprite;
    }
}
