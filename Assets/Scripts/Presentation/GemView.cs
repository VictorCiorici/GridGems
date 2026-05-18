using UnityEngine;
using GridGame.Domain;

namespace GridGame.Presentation
{
    /// <summary>
    /// Handles the visual representation of a gem.
    /// </summary>
    public class GemView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private GemEntity _gemEntity;

        /// <summary>
        /// Sets up the gem view with data and sprite.
        /// </summary>
        /// <param name="gemEntity">The domain gem entity.</param>
        /// <param name="gemSprite">The sprite to display.</param>
        public void Setup(GemEntity gemEntity, Sprite gemSprite)
        {
            _gemEntity = gemEntity;
            spriteRenderer.sprite = gemSprite;
            _gemEntity.OnGemFound += HandleGemFound;
        }

        private void HandleGemFound(GemEntity gem)
        {
            // The gem remains visible when found.
            // (Previously we disabled the GameObject here)
        }

        private void OnDestroy()
        {
            if (_gemEntity != null)
            {
                _gemEntity.OnGemFound -= HandleGemFound;
            }
        }
    }
}
