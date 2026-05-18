using System;
using System.Collections.Generic;

namespace GridGame.Domain
{
    /// <summary>
    /// Manages the grid logic, including cells and gems.
    /// Win condition evaluation is delegated to <see cref="IWinCondition"/> via the Application layer.
    /// ID generation is delegated to <see cref="IIdGenerator"/>.
    /// </summary>
    public class GridSystem
    {
        /// <summary>Width of the grid.</summary>
        public int Width { get; }

        /// <summary>Height of the grid.</summary>
        public int Height { get; }

        /// <summary>2D array representing the cells in the grid.</summary>
        public CellNode[,] Cells { get; }

        /// <summary>Read-only view of the active (not yet found) gems.</summary>
        public IReadOnlyList<GemEntity> ActiveGems => _activeGems;
        private readonly List<GemEntity> _activeGems = new List<GemEntity>();

        /// <summary>Total number of gems placed on the grid.</summary>
        public int TotalGemsCount { get; private set; }

        /// <summary>Number of gems that have been found.</summary>
        public int FoundGemsCount { get; private set; }

        /// <summary>Fired whenever any cell state changes.</summary>
        public event Action OnGridChanged;

        /// <summary>Fired when a gem is fully revealed. Used by the Application layer to check win conditions.</summary>
        public event Action<GemEntity> OnGemFound;

        private readonly IIdGenerator _idGenerator;

        /// <summary>
        /// Initializes a new <see cref="GridSystem"/>.
        /// </summary>
        /// <param name="width">Grid width. Must be &gt; 0.</param>
        /// <param name="height">Grid height. Must be &gt; 0.</param>
        /// <param name="idGenerator">Strategy for generating gem IDs.</param>
        public GridSystem(int width, int height, IIdGenerator idGenerator)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Grid width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Grid height must be greater than zero.");
            }

            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));

            Width  = width;
            Height = height;
            Cells  = new CellNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cells[x, y] = new CellNode(new GridCoordinate(x, y));
                    Cells[x, y].OnStateChanged += _ => OnGridChanged?.Invoke();
                }
            }
        }

        /// <summary>Gets the cell at the specified coordinates, or <c>null</c> if out of bounds.</summary>
        public CellNode GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return null;
            }

            return Cells[x, y];
        }

        /// <summary>
        /// Attempts to place a gem on the grid at the specified origin.
        /// </summary>
        /// <returns>A <see cref="PlacementResult"/> describing success or failure.</returns>
        public PlacementResult TryPlaceGem(int width, int height, GridCoordinate origin)
        {
            var targetCells = new List<CellNode>();

            for (int dx = 0; dx < width; dx++)
            {
                for (int dy = 0; dy < height; dy++)
                {
                    CellNode cell = GetCell(origin.X + dx, origin.Y + dy);
                    if (cell == null)
                    {
                        return PlacementResult.Fail("Gem extends outside grid bounds.");
                    }

                    if (cell.IsOccupied)
                    {
                        return PlacementResult.Fail("One or more cells are already occupied.");
                    }

                    targetCells.Add(cell);
                }
            }

            GemEntity gem = new GemEntity(_idGenerator.NewId(), width, height, targetCells);
            _activeGems.Add(gem);
            gem.OnGemFound += HandleGemFound;
            TotalGemsCount++;

            return PlacementResult.Ok(gem);
        }

        private void HandleGemFound(GemEntity gem)
        {
            _activeGems.Remove(gem);
            FoundGemsCount++;
            OnGridChanged?.Invoke();
            OnGemFound?.Invoke(gem);
        }
    }
}
