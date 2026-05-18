namespace GridGame.Domain
{
    /// <summary>
    /// Represents the visibility state of a grid cell.
    /// </summary>
    public enum CellState
    {
        /// <summary>
        /// The cell is covered and its content is hidden.
        /// </summary>
        Covered,

        /// <summary>
        /// The cell is revealed and its content is visible.
        /// </summary>
        Revealed
    }
}
