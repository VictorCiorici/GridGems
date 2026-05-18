using System;
using System.Collections.Generic;

namespace GridGame.Domain
{
    /// <summary>
    /// Represents a gem that occupies one or more cells on the grid.
    /// </summary>
    public class GemEntity
    {
        /// <summary>
        /// Unique identifier for the gem.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Width of the gem in cells.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the gem in cells.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The bottom-left grid coordinate of this gem, computed from its occupied cells.
        /// </summary>
        public GridCoordinate Origin { get; private set; }

        /// <summary>
        /// The list of cells this gem occupies.
        /// </summary>
        public IReadOnlyList<CellNode> OccupiedCells { get; private set; }

        /// <summary>
        /// Indicates whether all cells of this gem have been revealed.
        /// </summary>
        public bool IsFound { get; private set; }

        /// <summary>
        /// Triggered when all cells of the gem are revealed.
        /// </summary>
        public event Action<GemEntity> OnGemFound;

        private int _revealedCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="GemEntity"/> class
        /// and immediately binds it to its occupied cells.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <param name="width">Width in cells.</param>
        /// <param name="height">Height in cells.</param>
        /// <param name="cells">The cells this gem occupies.</param>
        public GemEntity(string id, int width, int height, List<CellNode> cells)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Id must not be null or empty.", nameof(id));
            }
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }
            if (cells == null || cells.Count == 0)
            {
                throw new ArgumentException("Cells must not be null or empty.", nameof(cells));
            }

            Id = id;
            Width = width;
            Height = height;

            BindCells(cells);
        }

        private void BindCells(List<CellNode> cells)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;

            foreach (var cell in cells)
            {
                cell.Occupy();
                cell.OnStateChanged += HandleCellStateChanged;

                if (cell.Coordinate.X < minX) minX = cell.Coordinate.X;
                if (cell.Coordinate.Y < minY) minY = cell.Coordinate.Y;
            }

            OccupiedCells = cells;
            Origin = new GridCoordinate(minX, minY);
        }

        private void HandleCellStateChanged(CellNode node)
        {
            if (IsFound)
            {
                return;
            }

            if (node.State == CellState.Revealed)
            {
                _revealedCount++;
            }

            if (_revealedCount < OccupiedCells.Count)
            {
                return;
            }

            IsFound = true;
            foreach (var cell in OccupiedCells)
            {
                cell.OnStateChanged -= HandleCellStateChanged;
            }

            OnGemFound?.Invoke(this);
        }
    }
}
