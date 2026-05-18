namespace GridGame.Controller
{
    /// <summary>
    /// Defines the mode in which gems are placed on the grid.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Gems are placed randomly using the gem collection.
        /// </summary>
        Procedural,

        /// <summary>
        /// Gems are placed based on a predefined LevelData asset.
        /// </summary>
        Predefined
    }
}
