using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="GemEntity"/> class.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <param name="width">Width in cells.</param>
        /// <param name="height">Height in cells.</param>
        public GemEntity(string id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;
            IsFound = false;
        }

        /// <summary>
        /// Initializes the gem with the cells it occupies.
        /// </summary>
        /// <param name="cells">The list of cells.</param>
        public void Initialize(List<CellNode> cells)
        {
            OccupiedCells = cells;
            foreach (var cell in cells)
            {
                cell.IsOccupied = true;
                cell.OnStateChanged += HandleCellStateChanged;
            }
        }

        private void HandleCellStateChanged(CellNode node)
        {
            if (IsFound) return;

            if (OccupiedCells.All(c => c.State == CellState.Revealed))
            {
                IsFound = true;
                foreach (var cell in OccupiedCells)
                {
                    cell.OnStateChanged -= HandleCellStateChanged;
                }
                OnGemFound?.Invoke(this);
            }
        }
    }
}
