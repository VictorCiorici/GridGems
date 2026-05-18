using UnityEngine;
using GridGame.Domain;
using GridGame.Presentation;
using GridGame.Config;
using System.Collections.Generic;

namespace GridGame.Controller
{
    /// <summary>
    /// The game mode regime.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Gems are placed randomly.
        /// </summary>
        Procedural,

        /// <summary>
        /// Gems are placed based on a LevelData asset.
        /// </summary>
        Predefined
    }

    /// <summary>
    /// Composition root that manages the game lifecycle and connects domain and view.
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
        /// Starts or restarts the game.
        /// </summary>
        public void StartNewGame()
        {
            // Reset UI
            uiController.ShowWinScreen(false);
            uiController.Setup(StartNewGame);

            if (gameMode == GameMode.Predefined && levelData != null)
            {
                _gridSystem = new Domain.GridSystem(levelData.gridWidth, levelData.gridHeight);
                
                foreach (var gem in levelData.gems)
                {
                    _gridSystem.TryPlaceGem(gem.width, gem.height, new GridCoordinate(gem.origin.x, gem.origin.y));
                }
            }
            else
            {
                _gridSystem = new Domain.GridSystem(defaultWidth, defaultHeight);
                
                // Procedural placement
                if (gemCollection != null)
                {
                    foreach (var gem in gemCollection.gemVisuals)
                    {
                        if (gem == null) continue;

                        bool placed = false;
                        int attempts = 0;
                        while (!placed && attempts < 100)
                        {
                            int rx = Random.Range(0, defaultWidth);
                            int ry = Random.Range(0, defaultHeight);

                            var result = _gridSystem.TryPlaceGem(gem.width, gem.height, new GridCoordinate(rx, ry));
                            if (result != null)
                            {
                                placed = true;
                            }
                            attempts++;
                        }
                    }
                }
            }

            // Initialize View
            gridView.Initialize(_gridSystem, gemCollection);

            // Subscribe to events
            _gridSystem.OnGridChanged += UpdateUI;
            _gridSystem.OnGameWon += HandleWin;

            UpdateUI();
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
            if (_gridSystem != null)
            {
                _gridSystem.OnGridChanged -= UpdateUI;
                _gridSystem.OnGameWon -= HandleWin;
            }
        }
    }
}
