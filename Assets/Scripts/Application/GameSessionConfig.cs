using UnityEngine;
using GridGame.Controller;
using GridGame.Config;

namespace GridGame.Application
{
    /// <summary>
    /// Static configuration for passing data between scenes and persisting player progress.
    /// </summary>
    public static class GameSessionConfig
    {
        private const string SaveLevelKey = "GridGame_CampaignLevel";
        private const string SaveDifficultyKey = "GridGame_Difficulty";

        /// <summary>
        /// The mode to launch the game with.
        /// </summary>
        public static GameMode Mode { get; set; } = GameMode.Procedural;

        /// <summary>
        /// The campaign data if playing in campaign mode.
        /// </summary>
        public static CampaignData CurrentCampaign { get; set; }

        /// <summary>
        /// The current level index within the campaign.
        /// </summary>
        public static int CurrentLevelIndex { get; set; } = 0;

        /// <summary>
        /// The current selected difficulty index from the DifficultySettings asset.
        /// </summary>
        public static int CurrentDifficultyIndex { get; set; } = 0;

        /// <summary>
        /// Saves the campaign progress level index to PlayerPrefs.
        /// </summary>
        public static void SaveCampaignProgress(int index)
        {
            CurrentLevelIndex = index;
            PlayerPrefs.SetInt(SaveLevelKey, index);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads the campaign progress level index from PlayerPrefs.
        /// </summary>
        public static int LoadCampaignProgress()
        {
            CurrentLevelIndex = PlayerPrefs.GetInt(SaveLevelKey, 0);
            return CurrentLevelIndex;
        }

        /// <summary>
        /// Saves the selected difficulty index to PlayerPrefs.
        /// </summary>
        public static void SaveDifficulty(int difficultyIndex)
        {
            CurrentDifficultyIndex = difficultyIndex;
            PlayerPrefs.SetInt(SaveDifficultyKey, difficultyIndex);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads the selected difficulty index from PlayerPrefs.
        /// </summary>
        public static int LoadDifficulty()
        {
            // Default to index 0 (which you can configure as Normal, Easy, etc. in the scriptable object)
            CurrentDifficultyIndex = PlayerPrefs.GetInt(SaveDifficultyKey, 0);
            return CurrentDifficultyIndex;
        }

        /// <summary>
        /// Resets the session config to default procedural mode.
        /// </summary>
        public static void Reset()
        {
            Mode = GameMode.Procedural;
            CurrentCampaign = null;
            CurrentLevelIndex = 0;
        }
    }
}
