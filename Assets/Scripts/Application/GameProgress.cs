namespace GridGame.Application
{
    /// <summary>
    /// Immutable snapshot of gem discovery progress.
    /// Passed to UI components so they receive named, typed data instead of raw integers.
    /// </summary>
    public readonly struct GameProgress
    {
        /// <summary>Number of gems found so far.</summary>
        public int Found { get; }

        /// <summary>Total gems placed on the grid.</summary>
        public int Total { get; }

        /// <summary>Completion percentage in the range [0, 1].</summary>
        public float Percentage => Total == 0 ? 0f : (float)Found / Total;

        /// <summary>
        /// Initializes a new <see cref="GameProgress"/> snapshot.
        /// </summary>
        public GameProgress(int found, int total)
        {
            Found = found;
            Total = total;
        }
    }
}
