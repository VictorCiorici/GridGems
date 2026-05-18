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

        /// <summary>
        /// Whether this gem can be rotated by 90 degrees.
        /// </summary>
        public bool canRotate;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (width < 1)  { width  = 1; UnityEngine.Debug.LogWarning($"GemVisualData '{name}': width clamped to 1.", this); }
            if (height < 1) { height = 1; UnityEngine.Debug.LogWarning($"GemVisualData '{name}': height clamped to 1.", this); }
            if (sprite == null) UnityEngine.Debug.LogWarning($"GemVisualData '{name}': Sprite is not assigned.", this);
        }
#endif
    }
}
