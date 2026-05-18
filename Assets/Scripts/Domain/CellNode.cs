using System;

namespace GridGame.Domain
{
    /// <summary>
    /// Represents a single node/cell in the grid.
    /// </summary>
    public class CellNode
    {
        /// <summary>
        /// The coordinate of this cell on the grid.
        /// </summary>
        public GridCoordinate Coordinate { get; }

        /// <summary>
        /// The current state of the cell.
        /// </summary>
        public CellState State { get; private set; }

        /// <summary>
        /// Indicates whether this cell is occupied by a gem.
        /// </summary>
        public bool IsOccupied { get; set; }

        /// <summary>
        /// Triggered when the state of the cell changes.
        /// </summary>
        public event Action<CellNode> OnStateChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellNode"/> class.
        /// </summary>
        /// <param name="coordinate">The coordinate of the cell.</param>
        public CellNode(GridCoordinate coordinate)
        {
            Coordinate = coordinate;
            State = CellState.Covered;
            IsOccupied = false;
        }

        /// <summary>
        /// Reveals the cell, changing its state and notifying subscribers.
        /// </summary>
        public void Reveal()
        {
            if (State == CellState.Revealed) return;

            State = CellState.Revealed;
            OnStateChanged?.Invoke(this);
        }
    }
}
