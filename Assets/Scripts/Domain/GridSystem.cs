using System;
using System.Collections.Generic;

namespace GridGame.Domain
{
    /// <summary>
    /// Manages the grid logic, including cells and gems.
    /// </summary>
    public class GridSystem
    {
        /// <summary>
        /// Width of the grid.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the grid.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 2D array representing the cells in the grid.
        /// </summary>
        public CellNode[,] Cells { get; }

        /// <summary>
        /// List of active gems on the grid.
        /// </summary>
        public List<GemEntity> ActiveGems { get; }

        /// <summary>
        /// Total number of gems placed on the grid.
        /// </summary>
        public int TotalGemsCount { get; private set; }

        /// <summary>
        /// Number of gems that have been found.
        /// </summary>
        public int FoundGemsCount { get; private set; }

        /// <summary>
        /// Triggered when any cell state changes or a gem is found.
        /// </summary>
        public event Action OnGridChanged;

        /// <summary>
        /// Triggered when all gems have been found.
        /// </summary>
        public event Action OnGameWon;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridSystem"/> class.
        /// </summary>
        /// <param name="width">Width of the grid.</param>
        /// <param name="height">Height of the grid.</param>
        public GridSystem(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new CellNode[width, height];
            ActiveGems = new List<GemEntity>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cells[x, y] = new CellNode(new GridCoordinate(x, y));
                    Cells[x, y].OnStateChanged += _ => OnGridChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Gets the cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The <see cref="CellNode"/> if within bounds; otherwise, null.</returns>
        public CellNode GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return null;
            return Cells[x, y];
        }

        /// <summary>
        /// Tries to place a gem on the grid at the specified origin.
        /// </summary>
        /// <param name="width">Width of the gem.</param>
        /// <param name="height">Height of the gem.</param>
        /// <param name="origin">The bottom-left coordinate for the gem.</param>
        /// <returns>The created <see cref="GemEntity"/> if successful; otherwise, null.</returns>
        public GemEntity TryPlaceGem(int width, int height, GridCoordinate origin)
        {
            List<CellNode> targetCells = new List<CellNode>();

            for (int dx = 0; dx < width; dx++)
            {
                for (int dy = 0; dy < height; dy++)
                {
                    int x = origin.X + dx;
                    int y = origin.Y + dy;

                    CellNode cell = GetCell(x, y);
                    if (cell == null || cell.IsOccupied)
                    {
                        return null; // Cannot place
                    }
                    targetCells.Add(cell);
                }
            }

            GemEntity gem = new GemEntity(Guid.NewGuid().ToString(), width, height);
            gem.Initialize(targetCells);
            ActiveGems.Add(gem);
            gem.OnGemFound += HandleGemFound;
            TotalGemsCount++;

            return gem;
        }

        private void HandleGemFound(GemEntity gem)
        {
            ActiveGems.Remove(gem);
            FoundGemsCount++;
            OnGridChanged?.Invoke();

            if (FoundGemsCount == TotalGemsCount)
            {
                OnGameWon?.Invoke();
            }
        }
    }
}
