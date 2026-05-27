using UnityEngine;
using UnityEngine.SceneManagement;
using GridGame.Domain;
using GridGame.Application;
using GridGame.Presentation;
using GridGame.Config;

namespace GridGame.Controller
{
    /// <summary>
    /// Unity bootstrap and composition root. Creates and wires all Application-layer services.
    /// Reacts to state changes and coordinates scene transitions.
    /// </summary>
    public class GamePresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridView gridView;
        [SerializeField] private GridInputHandler gridInputHandler;
        [SerializeField] private GameUIController uiController;

        [Header("Configuration")]
        [SerializeField] private GameMode gameMode = GameMode.Procedural;
        [SerializeField] private LevelData levelData;
        [SerializeField] private GemCollection gemCollection;

        [Header("Procedural Settings")]
        [SerializeField] private int defaultWidth = 6;
        [SerializeField] private int defaultHeight = 6;
        [SerializeField] private GridGame.Config.DifficultySettings difficultySettings;

        // Application services — created in Awake, never replaced
        private GameStateManager _stateManager;
        private RevealCellUseCase _revealCellUseCase;
        private StartNewGameUseCase _startNewGameUseCase;

        // SRP sub-components
        private LevelGeneratorFactory _levelGeneratorFactory;
        private GameUIAdapter _uiAdapter;

        // Tracks the active grid for progress queries
        private GridSystem _currentGrid;
        private System.Action<GridGame.Domain.GemEntity> _onGemFoundHandler;

        private void Awake()
        {
            if (difficultySettings == null)
            {
                UnityEngine.Debug.LogError("DifficultySettings is not assigned in GamePresenter!");
            }

            // 1. Create pure Domain/Application layer services
            _stateManager = new GameStateManager(difficultySettings);
            _revealCellUseCase = new RevealCellUseCase(_stateManager);
            _startNewGameUseCase = new StartNewGameUseCase(_stateManager, new AllGemsFoundWinCondition(), new GuidIdGenerator());

            // 2. Instantiate decoupled helpers
            _levelGeneratorFactory = new LevelGeneratorFactory(defaultWidth, defaultHeight, gemCollection);
            _uiAdapter = new GameUIAdapter(uiController, _stateManager);

            // 3. Connect event subscriptions
            _stateManager.OnStateChanged += OnGameStateChanged;
            _stateManager.OnMistakeMade += RefreshUI;
            _onGemFoundHandler = _ => RefreshUI();
            
            gridInputHandler.Setup(_revealCellUseCase);
            gridView.OnCellViewSpawned += gridInputHandler.RegisterCell;

            // 4. Setup persistence abstraction
            GameSessionConfig.Persistence = new PlayerPrefsPersistenceService();

            // 5. Load saved settings
            GameSessionConfig.LoadDifficulty();
            GameSessionConfig.LoadCampaignProgress();

            // 6. Bind UI Actions
            _uiAdapter.BindActions(StartNewGame, LoadNextLevel, ReturnToMainMenu);
        }

        private void Start()
        {
            StartNewGame();
        }

        /// <summary>
        /// Starts or restarts the game by executing the StartNewGame use case.
        /// </summary>
        public void StartNewGame()
        {
            UnsubscribeFromCurrentGrid();
            
            // Get current selected difficulty index
            int difficultyIndex = GameSessionConfig.CurrentDifficultyIndex;
            
            // Delegate level generator resolution to the factory
            var generator = _levelGeneratorFactory.CreateGenerator(gameMode, levelData);
            
            _currentGrid = _startNewGameUseCase.Execute(generator, difficultyIndex);
            _currentGrid.OnGemFound += _onGemFoundHandler;
            gridView.Initialize(_currentGrid, new GemSpriteResolver(gemCollection));
            RefreshUI();
        }

        private void OnGameStateChanged(GameState state) 
        {
            if (state == GameState.Won)
            {
                bool isCampaign = GameSessionConfig.Mode == GameMode.Predefined && GameSessionConfig.CurrentCampaign != null;
                if (isCampaign)
                {
                    // Save campaign progress when completing a level
                    int nextIndex = GameSessionConfig.CurrentLevelIndex + 1;
                    if (nextIndex < GameSessionConfig.CurrentCampaign.Levels.Count)
                    {
                        GameSessionConfig.SaveCampaignProgress(nextIndex);
                    }
                }
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            _uiAdapter.Refresh(_currentGrid);
        }

        private void LoadNextLevel()
        {
            // Level index was already incremented/saved in OnGameStateChanged, but let's sync index double-check
            int nextIndex = GameSessionConfig.CurrentLevelIndex;
            if (GameSessionConfig.CurrentCampaign != null && nextIndex < GameSessionConfig.CurrentCampaign.Levels.Count)
            {
                StartNewGame();
            }
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene("main");
        }

        private void UnsubscribeFromCurrentGrid()
        {
            if (_currentGrid != null)
            {
                _currentGrid.OnGemFound -= _onGemFoundHandler;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromCurrentGrid();

            if (_stateManager != null)
            {
                _stateManager.OnStateChanged -= OnGameStateChanged;
                _stateManager.OnMistakeMade -= RefreshUI;
            }

            if (gridView != null)
            {
                gridView.OnCellViewSpawned -= gridInputHandler.RegisterCell;
            }
        }
    }
}
