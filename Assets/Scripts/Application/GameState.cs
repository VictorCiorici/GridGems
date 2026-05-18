namespace GridGame.Application
{
    /// <summary>
    /// Represents the lifecycle state of the game session.
    /// All state transitions are managed by <see cref="GameStateManager"/>.
    /// </summary>
    public enum GameState
    {
        /// <summary>No game session is active.</summary>
        Idle,

        /// <summary>A game session is in progress and accepts player input.</summary>
        Playing,

        /// <summary>All win conditions have been satisfied.</summary>
        Won,

        /// <summary>Reserved for future lose conditions (e.g. timer expiry).</summary>
        Lost
    }
}
