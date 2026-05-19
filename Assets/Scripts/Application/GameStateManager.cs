using System;
using GridGame.Domain;

namespace GridGame.Application
{
    /// <summary>
    /// Owns the <see cref="GameState"/> machine, tracks mistakes/lives, and drives transitions.
    /// The single source of truth for what state the game is in.
    /// </summary>
    public class GameStateManager
    {
        /// <summary>The current game state.</summary>
        public GameState Current { get; private set; } = GameState.Idle;

        /// <summary>Number of mistakes made so far in the current session.</summary>
        public int MistakesMade { get; private set; }

        /// <summary>Maximum allowed mistakes. If negative, represents unlimited mistakes.</summary>
        public int MistakesAllowed { get; private set; }

        /// <summary>Fired whenever the state transitions to a new value.</summary>
        public event Action<GameState> OnStateChanged;

        /// <summary>Fired whenever a mistake is made (even if it does not lead to game loss).</summary>
        public event Action OnMistakeMade;

        private GridSystem _grid;
        private IWinCondition _winCondition;
        private readonly IDifficultyConfig _difficultyConfig;

        /// <summary>
        /// Initializes the GameStateManager.
        /// </summary>
        public GameStateManager(IDifficultyConfig difficultyConfig)
        {
            _difficultyConfig = difficultyConfig ?? throw new ArgumentNullException(nameof(difficultyConfig));
        }

        /// <summary>
        /// Begins a new game session with the given grid, win condition, and difficulty.
        /// Transitions state to <see cref="GameState.Playing"/>.
        /// </summary>
        public void StartGame(GridSystem grid, IWinCondition winCondition, int difficultyIndex)
        {
            if (_grid != null)
            {
                _grid.OnGemFound -= HandleGemFound;
            }

            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _winCondition = winCondition ?? throw new ArgumentNullException(nameof(winCondition));
            _grid.OnGemFound += HandleGemFound;

            MistakesMade = 0;
            MistakesAllowed = CalculateAllowedMistakes(grid, difficultyIndex);

            TransitionTo(GameState.Playing);
        }

        /// <summary>
        /// Records a mistake made by the player. If mistakes exceed allowed limit, transitions to Lost.
        /// </summary>
        public void RecordMistake()
        {
            if (Current != GameState.Playing)
            {
                return;
            }

            if (MistakesAllowed < 0)
            {
                MistakesMade++;
                OnMistakeMade?.Invoke();
                return; // Peaceful mode: unlimited mistakes
            }

            MistakesMade++;
            OnMistakeMade?.Invoke();
            if (MistakesMade > MistakesAllowed)
            {
                TransitionTo(GameState.Lost);
            }
        }

        private int CalculateAllowedMistakes(GridSystem grid, int difficultyIndex)
        {
            if (_difficultyConfig.IsUnlimited(difficultyIndex))
            {
                return -1; // -1 means unlimited
            }

            int totalCells = grid.Width * grid.Height;
            int gemCells = 0;

            foreach (var gem in grid.ActiveGems)
            {
                gemCells += gem.Width * gem.Height;
            }

            int emptyCells = totalCells - gemCells;
            if (emptyCells <= 0)
            {
                return 1;
            }

            float ratio = _difficultyConfig.GetMistakeRatio(difficultyIndex);
            int calculated = UnityEngine.Mathf.Max(1, UnityEngine.Mathf.RoundToInt(emptyCells * ratio));
            return calculated;
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

    // A helper class to resolve UnityEngine dependency safely in Domain/Application assemblies if needed.
    // Unity's Mathf is generally available in Unity projects since they compile together, but let's implement a simple Max/Round.
    internal static class Mathf
    {
        public static int Max(int a, int b) => a > b ? a : b;
        public static int RoundToInt(float value) => (int)System.Math.Round(value);
    }
}
