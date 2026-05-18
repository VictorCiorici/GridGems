namespace GridGame.Domain
{
    /// <summary>
    /// Defines the condition under which a game is considered won.
    /// Implement this to create alternative win conditions without modifying domain logic.
    /// </summary>
    public interface IWinCondition
    {
        /// <summary>
        /// Evaluates whether the win condition has been met for the given grid.
        /// </summary>
        /// <param name="grid">The current grid state.</param>
        /// <returns><c>true</c> if the game is won; otherwise <c>false</c>.</returns>
        bool IsWon(GridSystem grid);
    }
}
