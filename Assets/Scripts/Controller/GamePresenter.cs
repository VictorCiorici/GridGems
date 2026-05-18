using UnityEngine;
using GridGame.Domain;
using GridGame.Application;
using GridGame.Presentation;
using GridGame.Config;
using GridGame.Controller;

namespace GridGame.Controller
{
    /// <summary>
    /// Unity bootstrap and composition root. Creates and wires all Application-layer services.
    /// Contains no game logic — delegates entirely to use cases and reacts to state changes.
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

        // Application services — created in Awake, never replaced
        private GameStateManager _stateManager;
        private RevealCellUseCase _revealCellUseCase;
        private StartNewGameUseCase _startNewGameUseCase;

        // Tracks the active grid for progress queries
        private GridSystem _currentGrid;

        private void Awake()
        {
            _stateManager         = new GameStateManager();
            _revealCellUseCase    = new RevealCellUseCase(_stateManager);
            _startNewGameUseCase  = new StartNewGameUseCase(_stateManager, new AllGemsFoundWinCondition(), new GuidIdGenerator());

            _stateManager.OnStateChanged += OnGameStateChanged;
            gridInputHandler.Setup(_revealCellUseCase);
            gridView.OnCellViewSpawned += gridInputHandler.RegisterCell;
            uiController.Bind(StartNewGame);
        }

        private void Start() => StartNewGame();

        /// <summary>
        /// Starts or restarts the game by executing the StartNewGame use case.
        /// </summary>
        public void StartNewGame()
        {
            _currentGrid = _startNewGameUseCase.Execute(CreateGenerator());
            gridView.Initialize(_currentGrid, gemCollection);
            RefreshUI();
        }

        private ILevelGenerator CreateGenerator()
        {
            if (gameMode == GameMode.Predefined && levelData != null)
                return new PredefinedLevelGenerator(levelData);

            return new ProceduralLevelGenerator(defaultWidth, defaultHeight, gemCollection);
        }

        private void OnGameStateChanged(GameState _) => RefreshUI();

        private void RefreshUI()
        {
            var progress = new GameProgress(_currentGrid.FoundGemsCount, _currentGrid.TotalGemsCount);
            uiController.Refresh(_stateManager.Current, progress);
        }

        private void OnDestroy()
        {
            if (_stateManager != null)
                _stateManager.OnStateChanged -= OnGameStateChanged;

            if (gridView != null)
                gridView.OnCellViewSpawned -= gridInputHandler.RegisterCell;
        }
    }
}
