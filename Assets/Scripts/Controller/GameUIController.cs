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
        [SerializeField] private GameObject winScreenPanel;

        /// <summary>
        /// Binds the restart action to the restart button. Call once during setup.
        /// </summary>
        /// <param name="onRestart">Callback invoked when the restart button is pressed.</param>
        public void Bind(Action onRestart)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => onRestart?.Invoke());
        }

        /// <summary>
        /// Refreshes all UI elements to reflect the current game state and progress.
        /// </summary>
        /// <param name="state">The current <see cref="GameState"/>.</param>
        /// <param name="progress">The current gem discovery progress.</param>
        public void Refresh(GameState state, GameProgress progress)
        {
            if (progressText != null)
                progressText.text = $"Gems Found: {progress.Found} / {progress.Total}";

            if (winScreenPanel != null)
                winScreenPanel.SetActive(state == GameState.Won);
        }
    }
}
