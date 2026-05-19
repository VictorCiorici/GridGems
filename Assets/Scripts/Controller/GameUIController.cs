using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GridGame.Application;

namespace GridGame.Controller
{
    /// <summary>
    /// Manages all gameplay UI elements, driving visual updates based on clean state structures.
    /// </summary>
    public class GameUIController : MonoBehaviour
    {
        [Header("Status & Progress")]
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI livesText;

        [Header("Global Navigation")]
        [SerializeField] private Button menuButton;

        [Header("Win Screen")]
        [SerializeField] private GameObject winScreenPanel;
        [SerializeField] private Button nextLevelButton;

        [Header("Lose Screen")]
        [SerializeField] private GameObject loseScreenPanel;
        [SerializeField] private Button loseRestartButton;

        private Action _onRestart;
        private Action _onNextLevel;
        private Action _onMainMenu;

        /// <summary>
        /// Binds actions to their corresponding UI buttons.
        /// </summary>
        public void BindActions(Action onRestart, Action onNextLevel, Action onMainMenu)
        {
            _onRestart = onRestart;
            _onNextLevel = onNextLevel;
            _onMainMenu = onMainMenu;

            // Lose screen restart
            if (loseRestartButton != null)
            {
                loseRestartButton.onClick.RemoveAllListeners();
                loseRestartButton.onClick.AddListener(HandleRestartClicked);
            }

            // Next level in campaign
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(HandleNextLevelClicked);
            }

            // Global/HUD Back to main menu (Always available, or placed globally)
            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(HandleMainMenuClicked);
            }
        }

        private void HandleRestartClicked()
        {
            _onRestart?.Invoke();
        }

        private void HandleNextLevelClicked()
        {
            _onNextLevel?.Invoke();
        }

        private void HandleMainMenuClicked()
        {
            _onMainMenu?.Invoke();
        }

        /// <summary>
        /// Refreshes all UI components to accurately reflect the game progress and current state.
        /// </summary>
        public void Refresh(GameState state, GameProgress progress, bool isCampaign, bool hasNextLevel)
        {
            // Update Gem counter
            if (progressText != null)
            {
                progressText.text = $"Gems Found: {progress.Found} / {progress.Total}";
            }

            // Update Lives/Mistakes counter
            if (livesText != null)
            {
                if (progress.IsUnlimitedLives)
                {
                    livesText.text = "Lives: Infinite";
                }
                else
                {
                    int remaining = progress.MistakesAllowed - progress.MistakesMade;
                    // Keep it non-negative in UI
                    remaining = remaining < 0 ? 0 : remaining;
                    livesText.text = $"Lives: {remaining}";
                }
            }

            // Handle screens active state
            if (winScreenPanel != null)
            {
                winScreenPanel.SetActive(state == GameState.Won);
            }

            if (loseScreenPanel != null)
            {
                loseScreenPanel.SetActive(state == GameState.Lost);
            }

            // Handle Campaign flow button visibility
            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(state == GameState.Won && isCampaign && hasNextLevel);
            }
        }
    }
}
