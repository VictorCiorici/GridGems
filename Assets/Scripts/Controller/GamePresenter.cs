using UnityEngine;
using GridGame.Domain;
using GridGame.Presentation;
using GridGame.Config;

namespace GridGame.Controller
{
    /// <summary>
    /// Composition root that manages the game lifecycle and connects domain and view.
    /// Delegates level generation to <see cref="ILevelGenerator"/> implementations,
    /// keeping this class closed for modification when new game modes are added.
    /// </summary>
    public class GamePresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridView gridView;
        [SerializeField] private GameUIController uiController;

        [Header("Configuration")]
        [SerializeField] private GameMode gameMode = GameMode.Procedural;
        [SerializeField] private LevelData levelData;
        [SerializeField] private GemCollection gemCollection;

        [Header("Procedural Settings")]
        [SerializeField] private int defaultWidth = 6;
        [SerializeField] private int defaultHeight = 6;

        private Domain.GridSystem _gridSystem;

        private void Start()
        {
            StartNewGame();
        }

        /// <summary>
        /// Starts or restarts the game by building a fresh grid and re-initialising the view.
        /// </summary>
        public void StartNewGame()
        {
            uiController.ShowWinScreen(false);
            uiController.Setup(StartNewGame);

            UnsubscribeFromGrid();

            ILevelGenerator generator = CreateGenerator();
            _gridSystem = new Domain.GridSystem(generator.GridWidth, generator.GridHeight);
            generator.Populate(_gridSystem);

            gridView.Initialize(_gridSystem, gemCollection);

            _gridSystem.OnGridChanged += UpdateUI;
            _gridSystem.OnGameWon    += HandleWin;

            UpdateUI();
        }

        /// <summary>
        /// Creates the appropriate level generator based on the current <see cref="GameMode"/>.
        /// Add new modes here by extending <see cref="ILevelGenerator"/> and adding a case.
        /// </summary>
        private ILevelGenerator CreateGenerator()
        {
            if (gameMode == GameMode.Predefined && levelData != null)
                return new PredefinedLevelGenerator(levelData);

            return new ProceduralLevelGenerator(defaultWidth, defaultHeight, gemCollection);
        }

        private void UnsubscribeFromGrid()
        {
            if (_gridSystem == null) return;
            _gridSystem.OnGridChanged -= UpdateUI;
            _gridSystem.OnGameWon    -= HandleWin;
        }

        private void UpdateUI()
        {
            uiController.UpdateProgress(_gridSystem.FoundGemsCount, _gridSystem.TotalGemsCount);
        }

        private void HandleWin()
        {
            uiController.ShowWinScreen(true);
        }

        private void OnDestroy()
        {
            UnsubscribeFromGrid();
        }
    }
}
