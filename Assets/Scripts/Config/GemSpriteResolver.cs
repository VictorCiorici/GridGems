using UnityEngine;
using System;

namespace GridGame.Config
{
    /// <summary>
    /// Resolves gem sprites by querying visual data and applying dimension-swapping/rotation policy.
    /// Decouples visual representation rules from the raw collection asset.
    /// </summary>
    public class GemSpriteResolver : IGemSpriteResolver
    {
        private readonly GemCollection _gemCollection;

        /// <summary>
        /// Initializes a new instance of <see cref="GemSpriteResolver"/>.
        /// </summary>
        /// <param name="gemCollection">The source gem collection containing visuals.</param>
        public GemSpriteResolver(GemCollection gemCollection)
        {
            _gemCollection = gemCollection ?? throw new ArgumentNullException(nameof(gemCollection));
        }

        /// <inheritdoc/>
        public (Sprite sprite, bool needsRotation) GetSpriteForSize(int width, int height)
        {
            // 1. Check for exact match
            foreach (var config in _gemCollection.GemVisuals)
            {
                if (config != null && config.width == width && config.height == height)
                {
                    return (config.sprite, false);
                }
            }

            // 2. Check for swapped match (rotated)
            foreach (var config in _gemCollection.GemVisuals)
            {
                if (config != null && config.canRotate && config.width == height && config.height == width)
                {
                    return (config.sprite, true);
                }
            }

            // 3. Fallback to default
            return (_gemCollection.defaultVisual != null ? _gemCollection.defaultVisual.sprite : null, false);
        }
    }
}
