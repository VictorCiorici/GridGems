using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GridGame.Application;

namespace GridGame.Controller
{
    /// <summary>
    /// Manages all game UI. Accepts typed <see cref="GameState"/> and <see cref="GameProgress"/>
    /// instead of raw booleans and integers, making the contract explicit and self-documenting.
    /// </summary>
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private GameObject winScreenPanel;

        private Action _onRestart;
        private Action _onNextLevel;

        /// <summary>
        /// Binds the restart action to the restart button. Call once during setup.
        /// </summary>
        /// <param name="onRestart">Callback invoked when the restart button is pressed.</param>
        public void Bind(Action onRestart)
        {
            _onRestart = onRestart;
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(HandleRestartClicked);
        }

        public void BindNextLevel(Action onNextLevel)
        {
            _onNextLevel = onNextLevel;
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(HandleNextLevelClicked);
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

        /// <summary>
        /// Refreshes all UI elements to reflect the current game state and progress.
        /// </summary>
        public void Refresh(GameState state, GameProgress progress, bool isCampaign, bool hasNextLevel)
        {
            if (progressText != null)
            {
                progressText.text = $"Gems Found: {progress.Found} / {progress.Total}";
            }

            if (winScreenPanel != null)
            {
                winScreenPanel.SetActive(state == GameState.Won);
            }

            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(state == GameState.Won && isCampaign && hasNextLevel);
            }
        }
    }
}
