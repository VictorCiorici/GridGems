using UnityEngine;

namespace GridGame.Config
{
    /// <summary>
    /// Abstraction for resolving gem sprites by size.
    /// Allows GridView to depend on an interface rather than a concrete ScriptableObject.
    /// </summary>
    public interface IGemSpriteResolver
    {
        /// <summary>
        /// Gets the sprite and rotation flag for the given gem dimensions.
        /// </summary>
        /// <param name="width">Width of the gem in cells.</param>
        /// <param name="height">Height of the gem in cells.</param>
        /// <returns>A tuple of (sprite, needsRotation).</returns>
        (Sprite sprite, bool needsRotation) GetSpriteForSize(int width, int height);
    }
}
