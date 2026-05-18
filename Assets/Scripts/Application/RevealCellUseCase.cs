using System;
using GridGame.Domain;

namespace GridGame.Application
{
    /// <summary>
    /// Use case: reveal a single cell.
    /// Guards against execution when the game is not in the <see cref="GameState.Playing"/> state,
    /// providing a single interception point for all input before it reaches the domain.
    /// </summary>
    public class RevealCellUseCase
    {
        private readonly GameStateManager _stateManager;

        /// <summary>
        /// Initializes a new <see cref="RevealCellUseCase"/>.
        /// </summary>
        /// <param name="stateManager">The game state manager to consult before revealing.</param>
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

            node.Reveal();
        }
    }
}
