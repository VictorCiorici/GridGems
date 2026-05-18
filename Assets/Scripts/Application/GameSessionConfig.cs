using GridGame.Controller;
using GridGame.Config;

namespace GridGame.Application
{
    /// <summary>
    /// Static configuration for passing data between scenes.
    /// This holds the current game mode and campaign progress.
    /// </summary>
    public static class GameSessionConfig
    {
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
