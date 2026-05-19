using System;
using GridGame.Domain;

namespace GridGame.Application
{
    /// <summary>
    /// Use case: reveal a single cell.
    /// Guards execution when the game is not active, detects mistakes (revealing empty cells),
    /// and reports them to the <see cref="GameStateManager"/>.
    /// </summary>
    public class RevealCellUseCase
    {
        private readonly GameStateManager _stateManager;

        /// <summary>
        /// Initializes a new <see cref="RevealCellUseCase"/>.
        /// </summary>
        /// <param name="stateManager">The game state manager to consult and update.</param>
        public RevealCellUseCase(GameStateManager stateManager)
        {
            _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
        }

        /// <summary>
        /// Reveals the given cell if the game is currently in progress.
        /// </summary>
        /// <param name="node">The cell node to reveal.</param>
        public void Execute(CellNode node)
        {
            if (node == null)
            {
                return;
            }

            if (_stateManager.Current != GameState.Playing)
            {
                return;
            }

            // If already revealed, do nothing
            if (node.State == CellState.Revealed)
            {
                return;
            }

            // Perform the reveal
            node.Reveal();

            // If the cell was NOT occupied, it's a mistake!
            if (!node.IsOccupied)
            {
                _stateManager.RecordMistake();
            }
        }
    }
}
