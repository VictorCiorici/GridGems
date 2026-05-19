namespace GridGame.Application
{
    /// <summary>
    /// Immutable snapshot of gem discovery and mistake progress.
    /// Passed to UI components so they receive named, typed data.
    /// </summary>
    public readonly struct GameProgress
    {
        /// <summary>Number of gems found so far.</summary>
        public int Found { get; }

        /// <summary>Total gems placed on the grid.</summary>
        public int Total { get; }

        /// <summary>Number of mistakes made so far.</summary>
        public int MistakesMade { get; }

        /// <summary>Maximum number of mistakes allowed (negative or very high for unlimited).</summary>
        public int MistakesAllowed { get; }

        /// <summary>Whether the player has unlimited lives on the current difficulty.</summary>
        public bool IsUnlimitedLives => MistakesAllowed < 0;

        /// <summary>Completion percentage in the range [0, 1].</summary>
        public float Percentage
        {
            get
            {
                if (Total == 0)
                {
                    return 0f;
                }

                return (float)Found / Total;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GameProgress"/> snapshot.
        /// </summary>
        public GameProgress(int found, int total, int mistakesMade, int mistakesAllowed)
        {
            Found = found;
            Total = total;
            MistakesMade = mistakesMade;
            MistakesAllowed = mistakesAllowed;
        }
    }
}
