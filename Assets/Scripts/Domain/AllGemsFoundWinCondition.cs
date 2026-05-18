namespace GridGame.Domain
{
    /// <summary>
    /// Win condition satisfied when all gems on the grid have been found.
    /// </summary>
    public sealed class AllGemsFoundWinCondition : IWinCondition
    {
        /// <inheritdoc/>
        public bool IsWon(GridSystem grid) =>
            grid.TotalGemsCount > 0 && grid.FoundGemsCount == grid.TotalGemsCount;
    }
}
