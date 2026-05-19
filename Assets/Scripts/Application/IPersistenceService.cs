namespace GridGame.Application
{
    /// <summary>
    /// Defines an abstraction for persistent key-value storage.
    /// Allows the Application layer to load and save data without being coupled to Unity engines like PlayerPrefs.
    /// </summary>
    public interface IPersistenceService
    {
        /// <summary>
        /// Saves an integer value associated with the specified key.
        /// </summary>
        /// <param name="key">The unique storage key.</param>
        /// <param name="value">The integer value to store.</param>
        void SetInt(string key, int value);

        /// <summary>
        /// Retrieves an integer value associated with the specified key, returning a default value if not found.
        /// </summary>
        /// <param name="key">The unique storage key.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The stored integer value, or <paramref name="defaultValue"/>.</returns>
        int GetInt(string key, int defaultValue = 0);

        /// <summary>
        /// Writes all modified preferences to disk/storage immediately.
        /// </summary>
        void Save();
    }
}
