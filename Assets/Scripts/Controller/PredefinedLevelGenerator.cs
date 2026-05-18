using System;
using GridGame.Domain;
using GridGame.Config;

namespace GridGame.Controller
{
    /// <summary>
    /// Places gems on the grid exactly as defined by a LevelData asset.
    /// </summary>
    public class PredefinedLevelGenerator : ILevelGenerator
    {
        private readonly LevelData _levelData;

        /// <inheritdoc/>
        public int GridWidth => _levelData.gridWidth;

        /// <inheritdoc/>
        public int GridHeight => _levelData.gridHeight;

        /// <summary>
        /// Initializes a new instance of <see cref="PredefinedLevelGenerator"/>.
        /// </summary>
        /// <param name="levelData">The level configuration asset.</param>
        public PredefinedLevelGenerator(LevelData levelData)
        {
            _levelData = levelData ?? throw new ArgumentNullException(nameof(levelData));
        }

        /// <inheritdoc/>
        public void Populate(GridSystem gridSystem)
        {
            foreach (var gem in _levelData.gems)
            {
                gridSystem.TryPlaceGem(gem.width, gem.height, new GridCoordinate(gem.origin.x, gem.origin.y));
            }
        }
    }
}
