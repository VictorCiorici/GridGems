using UnityEngine;
using System.Collections.Generic;

namespace GridGame.Config
{
    /// <summary>
    /// Holds a collection of GemVisualData assets and resolves sprites by gem size.
    /// Implements <see cref="IGemSpriteResolver"/> so that consumers depend on the interface, not the asset.
    /// </summary>
    [CreateAssetMenu(fileName = "NewGemCollection", menuName = "GridGame/GemCollection")]
    public class GemCollection : ScriptableObject
    {
        [SerializeField]
        private List<GemVisualData> gemVisuals = new List<GemVisualData>();

        /// <summary>
        /// Read-only access to the gem visual data list.
        /// </summary>
        public IReadOnlyList<GemVisualData> GemVisuals => gemVisuals;

        /// <summary>
        /// The default visual to use if no size match is found.
        /// </summary>
        public GemVisualData defaultVisual;

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < gemVisuals.Count; i++)
            {
                if (gemVisuals[i] == null)
                {
                    UnityEngine.Debug.LogWarning($"GemCollection '{name}': entry [{i}] is null.", this);
                    continue;
                }
                for (int j = i + 1; j < gemVisuals.Count; j++)
                {
                    var a = gemVisuals[i];
                    var b = gemVisuals[j];
                    if (b != null && a.width == b.width && a.height == b.height)
                    {
                        UnityEngine.Debug.LogWarning($"GemCollection '{name}': duplicate size {a.width}x{a.height} at indices [{i}] and [{j}].", this);
                    }
                }
            }
        }
#endif
    }
}
