using System;
using UnityEngine;
using UnityEngine.EventSystems;
using GridGame.Domain;

namespace GridGame.Presentation
{
    /// <summary>
    /// Handles the visual representation and interaction for a single grid cell.
    /// Raises <see cref="OnCellClicked"/> on click instead of calling domain directly,
    /// allowing a controller to intercept and apply game-state checks.
    /// </summary>
    public class CellView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private SpriteRenderer coverRenderer;

        private CellNode _node;

        /// <summary>
        /// Raised when the player clicks this cell. Passes the underlying <see cref="CellNode"/>.
        /// Subscribe in a controller to handle the reveal logic.
        /// </summary>
        public event Action<CellNode> OnCellClicked;

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
        /// Handles the pointer click event. Raises <see cref="OnCellClicked"/> for external handling.
        /// </summary>
        /// <param name="eventData">Pointer event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_node != null)
                OnCellClicked?.Invoke(_node);
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
