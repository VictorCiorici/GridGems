namespace GridGame.Application
{
    /// <summary>
    /// Provides configuration rules for different difficulty settings.
    /// Injected to decouple Application logic from Unity ScriptableObjects.
    /// </summary>
    public interface IDifficultyConfig
    {
        /// <summary>
        /// Returns the name of the difficulty at the given index.
        /// </summary>
        string GetName(int difficultyIndex);

        /// <summary>
        /// Returns the total number of defined difficulties.
        /// </summary>
        int GetDifficultyCount();

        /// <summary>
        /// Returns true if the difficulty has no limits on mistakes.
        /// </summary>
        bool IsUnlimited(int difficultyIndex);

        /// <summary>
        /// Returns the ratio (e.g., 0.0 to 1.0) of empty cells allowed to be revealed as mistakes.
        /// </summary>
        float GetMistakeRatio(int difficultyIndex);
    }
}
