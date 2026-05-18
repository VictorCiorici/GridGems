using UnityEngine;
using System.Collections.Generic;

namespace GridGame.Config
{
    /// <summary>
    /// Holds a collection of GemVisualData assets.
    /// </summary>
    [CreateAssetMenu(fileName = "NewGemCollection", menuName = "GridGame/GemCollection")]
    public class GemCollection : ScriptableObject
    {
        /// <summary>
        /// The list of specific gem visuals.
        /// </summary>
        public List<GemVisualData> gemVisuals = new List<GemVisualData>();

        /// <summary>
        /// The default visual to use if no size match is found.
        /// </summary>
        public GemVisualData defaultVisual;

        /// <summary>
        /// Gets the sprite for a specific size.
        /// </summary>
        /// <param name="width">Width in cells.</param>
        /// <param name="height">Height in cells.</param>
        /// <returns>The sprite if found; otherwise, the default sprite.</returns>
        public Sprite GetSpriteForSize(int width, int height)
        {
            foreach (var config in gemVisuals)
            {
                if (config != null && config.width == width && config.height == height)
                {
                    return config.sprite;
                }
            }
            return defaultVisual != null ? defaultVisual.sprite : null;
        }
    }
}
