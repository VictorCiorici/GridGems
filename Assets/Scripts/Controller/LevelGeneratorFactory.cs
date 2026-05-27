using GridGame.Application;
using GridGame.Config;

namespace GridGame.Controller
{
    /// <summary>
    /// Factory responsible for determining and instantiating the appropriate <see cref="ILevelGenerator"/> based on current game configuration.
    /// Decouples the decision of how a level is created from the main game coordination loop.
    /// </summary>
    public class LevelGeneratorFactory
    {
        private readonly int _defaultWidth;
        private readonly int _defaultHeight;
        private readonly GemCollection _gemCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelGeneratorFactory"/> class.
        /// </summary>
        /// <param name="defaultWidth">Fallback default width of the generated grid.</param>
        /// <param name="defaultHeight">Fallback default height of the generated grid.</param>
        /// <param name="gemCollection">The standard collection of gem metadata definitions.</param>
        public LevelGeneratorFactory(int defaultWidth, int defaultHeight, GemCollection gemCollection)
        {
            _defaultWidth = defaultWidth;
            _defaultHeight = defaultHeight;
            _gemCollection = gemCollection;
        }

        /// <summary>
        /// Resolves and instantiates the correct <see cref="ILevelGenerator"/> implementation.
        /// Checks active runtime session data first (e.g. Campaign levels), then falls back to Unity Inspector parameters.
        /// </summary>
        /// <param name="fallbackGameMode">The standalone game mode fallback.</param>
        /// <param name="fallbackLevelData">The standalone predefined level asset fallback.</param>
        /// <returns>A valid <see cref="ILevelGenerator"/> strategy ready to populate the grid.</returns>
        public ILevelGenerator CreateGenerator(GameMode fallbackGameMode, LevelData fallbackLevelData)
        {
            // 1. Check if we are running in Campaign mode from Main Menu
            if (GameSessionConfig.Mode == GameMode.Predefined && GameSessionConfig.CurrentCampaign != null)
            {
                var campaign = GameSessionConfig.CurrentCampaign;
                int index = GameSessionConfig.CurrentLevelIndex;

                if (index >= 0 && index < campaign.Levels.Count)
                {
                    if (campaign.Levels[index] != null)
                    {
                        return new PredefinedLevelGenerator(campaign.Levels[index]);
                    }
                }
            }

            // 2. Fallback or Standalone scene play: use editor settings
            if (fallbackGameMode == GameMode.Predefined && fallbackLevelData != null)
            {
                return new PredefinedLevelGenerator(fallbackLevelData);
            }

            return new ProceduralLevelGenerator(_defaultWidth, _defaultHeight, _gemCollection);
        }
    }
}
