using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using GridGame.Application;
using GridGame.Config;

namespace GridGame.Controller
{
    /// <summary>
    /// Handles interactions, persistence loading, and difficulty configuration in the Main Menu scene.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Global Settings")]
        [SerializeField] private DifficultySettings difficultySettings;

        [Header("Campaign Configuration")]
        [SerializeField] private CampaignData campaignData;

        [Header("UI Outputs")]
        [SerializeField] private TextMeshProUGUI difficultyDisplay;
        [SerializeField] private TextMeshProUGUI campaignProgressDisplay;

        private void Start()
        {
            if (difficultySettings == null)
            {
                Debug.LogError("MainMenuController: Difficulty Settings is not assigned!", this);
            }

            // Sync values from PlayerPrefs
            GameSessionConfig.LoadDifficulty();
            GameSessionConfig.LoadCampaignProgress();

            UpdateUI();
        }

        /// <summary>
        /// Starts a random game by loading the game scene with procedural mode.
        /// </summary>
        public void PlayRandom()
        {
            GameSessionConfig.Reset();
            GameSessionConfig.Mode = GameMode.Procedural;
            SceneManager.LoadScene("Playground");
        }

        /// <summary>
        /// Starts the campaign by loading the game scene from the saved level index.
        /// </summary>
        public void PlayCampaign()
        {
            if (campaignData == null)
            {
                Debug.LogError("MainMenuController: Campaign Data is not assigned!", this);
                return;
            }

            if (campaignData.levels.Count == 0)
            {
                Debug.LogError("MainMenuController: Campaign has no levels!", this);
                return;
            }

            // Sync and clamp level index
            int savedIndex = GameSessionConfig.LoadCampaignProgress();
            if (savedIndex >= campaignData.levels.Count)
            {
                savedIndex = campaignData.levels.Count - 1;
            }

            GameSessionConfig.Reset();
            GameSessionConfig.Mode = GameMode.Predefined;
            GameSessionConfig.CurrentCampaign = campaignData;
            GameSessionConfig.CurrentLevelIndex = savedIndex;

            SceneManager.LoadScene("Playground");
        }

        /// <summary>
        /// Cycles the selected difficulty level and saves it.
        /// </summary>
        public void CycleDifficulty()
        {
            if (difficultySettings == null || difficultySettings.GetDifficultyCount() == 0) return;

            int current = GameSessionConfig.CurrentDifficultyIndex;
            int nextIndex = (current + 1) % difficultySettings.GetDifficultyCount();

            GameSessionConfig.SaveDifficulty(nextIndex);
            UpdateUI();
        }

        /// <summary>
        /// Direct setter for difficulty. Perfect for Dropdown bindings.
        /// </summary>
        public void SetDifficulty(int difficultyIndex)
        {
            if (difficultySettings == null || difficultySettings.GetDifficultyCount() == 0) return;

            difficultyIndex = UnityEngine.Mathf.Clamp(difficultyIndex, 0, difficultySettings.GetDifficultyCount() - 1);
            GameSessionConfig.SaveDifficulty(difficultyIndex);
            UpdateUI();
        }

        /// <summary>
        /// Resets campaign progress back to Level 1.
        /// </summary>
        public void ResetCampaignProgress()
        {
            GameSessionConfig.SaveCampaignProgress(0);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (difficultyDisplay != null && difficultySettings != null)
            {
                int currentDiff = GameSessionConfig.CurrentDifficultyIndex;
                string diffName = difficultySettings.GetName(currentDiff);
                difficultyDisplay.text = $"Difficulty: {diffName}";
            }

            if (campaignProgressDisplay != null)
            {
                int currentLevelIndex = GameSessionConfig.CurrentLevelIndex;
                int totalLevels = campaignData != null ? campaignData.levels.Count : 0;
                campaignProgressDisplay.text = $"Campaign Level: {currentLevelIndex + 1} / {totalLevels}";
            }
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}
