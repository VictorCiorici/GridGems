using UnityEngine;
using GridGame.Domain;
using GridGame.Application;

namespace GridGame.Presentation
{
    /// <summary>
    /// Bridges player input (cell clicks) to the Application layer.
    /// Completely decouples <see cref="GridView"/> from domain interaction.
    /// </summary>
    public class GridInputHandler : MonoBehaviour
    {
        private RevealCellUseCase _revealCellUseCase;

        /// <summary>
        /// Binds the use case that will be called when a cell is clicked.
        /// </summary>
        public void Setup(RevealCellUseCase revealCellUseCase)
        {
            _revealCellUseCase = revealCellUseCase;
        }

        /// <summary>
        /// Subscribes to a <see cref="CellView"/>'s click event.
        /// Call this for every cell view spawned by <see cref="GridView"/>.
        /// </summary>
        public void RegisterCell(CellView cellView)
        {
            cellView.OnCellClicked += HandleCellClicked;
        }

        private void HandleCellClicked(CellNode node)
        {
            _revealCellUseCase?.Execute(node);
        }
    }
}
