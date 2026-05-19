using UnityEngine;
using GridGame.Application;

namespace GridGame.Controller
{
    /// <summary>
    /// Concrete implementation of the <see cref="IPersistenceService"/> interface using Unity's native <see cref="PlayerPrefs"/>.
    /// </summary>
    public class PlayerPrefsPersistenceService : IPersistenceService
    {
        /// <inheritdoc/>
        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        /// <inheritdoc/>
        public int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        /// <inheritdoc/>
        public void Save()
        {
            PlayerPrefs.Save();
        }
    }
}
