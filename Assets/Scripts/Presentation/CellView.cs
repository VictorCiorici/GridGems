using System;
using UnityEngine;
using UnityEngine.EventSystems;
using GridGame.Domain;

namespace GridGame.Presentation
{
    /// <summary>
    /// Handles the visual representation and interaction for a single grid cell.
    /// </summary>
    public class CellView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private SpriteRenderer coverRenderer;

        private CellNode _node;

        /// <summary>
        /// Sets up the view with a corresponding domain node.
        /// </summary>
        /// <param name="node">The domain cell node.</param>
        public void Setup(CellNode node)
        {
            _node = node;
            _node.OnStateChanged += UpdateVisuals;
            UpdateVisuals(_node);
        }

        /// <summary>
        /// Handles the pointer click event to reveal the cell.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            _node?.Reveal();
        }

        private void UpdateVisuals(CellNode node)
        {
            if (node.State == CellState.Covered)
            {
                coverRenderer.gameObject.SetActive(true);
            }
            else if (node.State == CellState.Revealed)
            {
                coverRenderer.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_node != null)
            {
                _node.OnStateChanged -= UpdateVisuals;
            }
        }
    }
}
