namespace GridGame.Domain
{
    /// <summary>
    /// Represents a 2D coordinate on the game grid.
    /// </summary>
    public struct GridCoordinate
    {
        /// <summary>
        /// The X coordinate (column).
        /// </summary>
        public int X { get; }

        /// <summary>
        /// The Y coordinate (row).
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridCoordinate"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public GridCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
