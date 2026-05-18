using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GridGame.Controller
{
    /// <summary>
    /// Manages the game UI elements like progress text and win screen.
    /// </summary>
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Button restartButton;
        [SerializeField] private GameObject winScreenPanel;

        /// <summary>
        /// Sets up the UI with a restart callback.
        /// </summary>
        /// <param name="onRestart">Callback action for restart button.</param>
        public void Setup(Action onRestart)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => onRestart?.Invoke());
        }

        /// <summary>
        /// Updates the progress text.
        /// </summary>
        /// <param name="current">Current gems found.</param>
        /// <param name="total">Total gems to find.</param>
        public void UpdateProgress(int current, int total)
        {
            if (progressText != null)
            {
                progressText.text = $"Gems Found: {current} / {total}";
            }
        }

        /// <summary>
        /// Shows or hides the win screen.
        /// </summary>
        /// <param name="isVisible">True to show, false to hide.</param>
        public void ShowWinScreen(bool isVisible)
        {
            if (winScreenPanel != null)
            {
                winScreenPanel.SetActive(isVisible);
            }
        }
    }
}
