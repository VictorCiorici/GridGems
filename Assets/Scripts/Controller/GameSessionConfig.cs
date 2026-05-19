using GridGame.Config;
using GridGame.Application;

namespace GridGame.Controller
{
    /// <summary>
    /// Static configuration for passing data between scenes and persisting player progress.
    /// </summary>
    public static class GameSessionConfig
    {
        private const string SaveLevelKey = "GridGame_CampaignLevel";
        private const string SaveDifficultyKey = "GridGame_Difficulty";

        /// <summary>
        /// The persistence service to use for saving and loading player progress.
        /// Must be initialized at the application entry points (Composition Roots).
        /// </summary>
        public static IPersistenceService Persistence { get; set; }

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
        /// Saves the campaign progress level index to the persistence store.
        /// </summary>
        public static void SaveCampaignProgress(int index)
        {
            CurrentLevelIndex = index;
            Persistence?.SetInt(SaveLevelKey, index);
            Persistence?.Save();
        }

        /// <summary>
        /// Loads the campaign progress level index from the persistence store.
        /// </summary>
        public static int LoadCampaignProgress()
        {
            if (Persistence != null)
            {
                CurrentLevelIndex = Persistence.GetInt(SaveLevelKey, 0);
            }
            return CurrentLevelIndex;
        }

        /// <summary>
        /// Saves the selected difficulty index to the persistence store.
        /// </summary>
        public static void SaveDifficulty(int difficultyIndex)
        {
            CurrentDifficultyIndex = difficultyIndex;
            Persistence?.SetInt(SaveDifficultyKey, difficultyIndex);
            Persistence?.Save();
        }

        /// <summary>
        /// Loads the selected difficulty index from the persistence store.
        /// </summary>
        public static int LoadDifficulty()
        {
            if (Persistence != null)
            {
                CurrentDifficultyIndex = Persistence.GetInt(SaveDifficultyKey, 0);
            }
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
