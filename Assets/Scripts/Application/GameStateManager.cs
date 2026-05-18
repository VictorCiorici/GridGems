using System;
using GridGame.Domain;

namespace GridGame.Application
{
    /// <summary>
    /// Owns the <see cref="GameState"/> machine and drives transitions in response to domain events.
    /// The single source of truth for what state the game is in.
    /// </summary>
    public class GameStateManager
    {
        /// <summary>The current game state.</summary>
        public GameState Current { get; private set; } = GameState.Idle;

        /// <summary>Fired whenever the state transitions to a new value.</summary>
        public event Action<GameState> OnStateChanged;

        private GridSystem _grid;
        private IWinCondition _winCondition;

        /// <summary>
        /// Begins a new game session with the given grid and win condition.
        /// Transitions state to <see cref="GameState.Playing"/>.
        /// </summary>
        public void StartGame(GridSystem grid, IWinCondition winCondition)
        {
            if (_grid != null)
            {
                _grid.OnGemFound -= HandleGemFound;
            }

            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _winCondition = winCondition ?? throw new ArgumentNullException(nameof(winCondition));
            _grid.OnGemFound += HandleGemFound;

            TransitionTo(GameState.Playing);
        }

        private void HandleGemFound(GemEntity _)
        {
            if (Current != GameState.Playing)
            {
                return;
            }

            if (_winCondition.IsWon(_grid))
            {
                TransitionTo(GameState.Won);
            }
        }

        private void TransitionTo(GameState newState)
        {
            if (Current == newState)
            {
                return;
            }

            Current = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
