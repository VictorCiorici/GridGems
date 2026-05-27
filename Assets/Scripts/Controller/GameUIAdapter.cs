using System;
using GridGame.Application;
using GridGame.Domain;

namespace GridGame.Controller
{
    /// <summary>
    /// Adapter responsible for mapping game domain and state data to the UI presentation layer.
    /// Decouples UI refreshing details and state mappings from the main game flow presenter.
    /// </summary>
    public class GameUIAdapter
    {
        private readonly GameUIController _uiController;
        private readonly GameStateManager _stateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameUIAdapter"/> class.
        /// </summary>
        /// <param name="uiController">The visual UI controller displaying game elements.</param>
        /// <param name="stateManager">The domain/application game state manager tracking mistakes and states.</param>
        public GameUIAdapter(GameUIController uiController, GameStateManager stateManager)
        {
            _uiController = uiController ?? throw new ArgumentNullException(nameof(uiController));
            _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
        }

        /// <summary>
        /// Binds user actions from UI elements (like buttons) to high-level game logic.
        /// </summary>
        /// <param name="onRestart">Action to invoke for restarting the level.</param>
        /// <param name="onNextLevel">Action to invoke for loading the next level in a campaign.</param>
        /// <param name="onMainMenu">Action to invoke for returning to the main menu.</param>
        public void BindActions(Action onRestart, Action onNextLevel, Action onMainMenu)
        {
            _uiController.BindActions(onRestart, onNextLevel, onMainMenu);
        }

        /// <summary>
        /// Synthesizes state snapshots from the domain entities and triggers UI refresh.
        /// </summary>
        /// <param name="gridSystem">The active grid system containing board progress state.</param>
        public void Refresh(GridSystem gridSystem)
        {
            if (gridSystem == null)
            {
                return;
            }

            var progress = new GameProgress(
                gridSystem.FoundGemsCount,
                gridSystem.TotalGemsCount,
                _stateManager.MistakesMade,
                _stateManager.MistakesAllowed
            );

            bool isCampaign = GameSessionConfig.Mode == GameMode.Predefined && GameSessionConfig.CurrentCampaign != null;
            bool hasNextLevel = false;

            if (isCampaign)
            {
                hasNextLevel = GameSessionConfig.CurrentLevelIndex + 1 < GameSessionConfig.CurrentCampaign.Levels.Count;
            }

            _uiController.Refresh(_stateManager.Current, progress, isCampaign, hasNextLevel);
        }
    }
}
