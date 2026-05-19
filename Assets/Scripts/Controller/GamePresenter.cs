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

        // Tracks the active grid for progress queries
        private GridSystem _currentGrid;
        private System.Action<GridGame.Domain.GemEntity> _onGemFoundHandler;

        private void Awake()
        {
            if (difficultySettings == null)
            {
                UnityEngine.Debug.LogError("DifficultySettings is not assigned in GamePresenter!");
            }

            _stateManager = new GameStateManager(difficultySettings);
            _revealCellUseCase = new RevealCellUseCase(_stateManager);
            _startNewGameUseCase = new StartNewGameUseCase(_stateManager, new AllGemsFoundWinCondition(), new GuidIdGenerator());

            _stateManager.OnStateChanged += OnGameStateChanged;
            _stateManager.OnMistakeMade += RefreshUI;
            _onGemFoundHandler = _ => RefreshUI();
            gridInputHandler.Setup(_revealCellUseCase);
            gridView.OnCellViewSpawned += gridInputHandler.RegisterCell;

            // Load saved settings
            GameSessionConfig.LoadDifficulty();
            GameSessionConfig.LoadCampaignProgress();

            uiController.BindActions(StartNewGame, LoadNextLevel, ReturnToMainMenu);
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
            
            _currentGrid = _startNewGameUseCase.Execute(CreateGenerator(), difficultyIndex);
            _currentGrid.OnGemFound += _onGemFoundHandler;
            gridView.Initialize(_currentGrid, gemCollection);
            RefreshUI();
        }

        private ILevelGenerator CreateGenerator()
        {
            // 1. Check if we are running in Campaign mode from Main Menu
            if (GameSessionConfig.Mode == GameMode.Predefined && GameSessionConfig.CurrentCampaign != null)
            {
                var campaign = GameSessionConfig.CurrentCampaign;
                int index = GameSessionConfig.CurrentLevelIndex;
                
                if (index >= 0 && index < campaign.levels.Count)
                {
                    if (campaign.levels[index] != null)
                    {
                        return new PredefinedLevelGenerator(campaign.levels[index]);
                    }
                    UnityEngine.Debug.LogWarning($"GamePresenter: Level at index {index} is null in campaign. Falling back.", this);
                }
            }

            // 2. Fallback or Standalone scene play: use inspector settings
            if (gameMode == GameMode.Predefined && levelData != null)
            {
                return new PredefinedLevelGenerator(levelData);
            }

            return new ProceduralLevelGenerator(defaultWidth, defaultHeight, gemCollection);
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
                    if (nextIndex < GameSessionConfig.CurrentCampaign.levels.Count)
                    {
                        GameSessionConfig.SaveCampaignProgress(nextIndex);
                    }
                }
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            if (_currentGrid == null) return;
            var progress = new GameProgress(
                _currentGrid.FoundGemsCount, 
                _currentGrid.TotalGemsCount, 
                _stateManager.MistakesMade, 
                _stateManager.MistakesAllowed
            );
            
            bool isCampaign = GameSessionConfig.Mode == GameMode.Predefined && GameSessionConfig.CurrentCampaign != null;
            bool hasNextLevel = false;
            
            if (isCampaign)
            {
                hasNextLevel = GameSessionConfig.CurrentLevelIndex + 1 < GameSessionConfig.CurrentCampaign.levels.Count;
            }

            uiController.Refresh(_stateManager.Current, progress, isCampaign, hasNextLevel);
        }

        private void LoadNextLevel()
        {
            // Level index was already incremented/saved in OnGameStateChanged, but let's sync index double-check
            int nextIndex = GameSessionConfig.CurrentLevelIndex;
            if (GameSessionConfig.CurrentCampaign != null && nextIndex < GameSessionConfig.CurrentCampaign.levels.Count)
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
