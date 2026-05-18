using System;
using UnityEngine;
using GridGame.Domain;
using GridGame.Config;

namespace GridGame.Controller
{
    /// <summary>
    /// Randomly places gems from a GemCollection onto the grid.
    /// Each gem has a 50% chance to be rotated if its CanRotate flag is set.
    /// </summary>
    public class ProceduralLevelGenerator : ILevelGenerator
    {
        private const int MaxPlacementAttempts = 100;

        private readonly GemCollection _gemCollection;

        /// <inheritdoc/>
        public int GridWidth { get; }

        /// <inheritdoc/>
        public int GridHeight { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ProceduralLevelGenerator"/>.
        /// </summary>
        /// <param name="gridWidth">Width of the grid to generate.</param>
        /// <param name="gridHeight">Height of the grid to generate.</param>
        /// <param name="gemCollection">The collection of gems to place.</param>
        public ProceduralLevelGenerator(int gridWidth, int gridHeight, GemCollection gemCollection)
        {
            if (gridWidth <= 0) throw new ArgumentOutOfRangeException(nameof(gridWidth));
            if (gridHeight <= 0) throw new ArgumentOutOfRangeException(nameof(gridHeight));

            GridWidth = gridWidth;
            GridHeight = gridHeight;
            _gemCollection = gemCollection;
        }

        /// <inheritdoc/>
        public void Populate(GridSystem gridSystem)
        {
            if (_gemCollection == null) return;

            foreach (var gem in _gemCollection.GemVisuals)
            {
                if (gem == null) continue;

                bool shouldRotate = gem.canRotate && UnityEngine.Random.value > 0.5f;
                int width  = shouldRotate ? gem.height : gem.width;
                int height = shouldRotate ? gem.width  : gem.height;

                bool placed = false;
                int attempts = 0;
                while (!placed && attempts < MaxPlacementAttempts)
                {
                    int rx = UnityEngine.Random.Range(0, GridWidth);
                    int ry = UnityEngine.Random.Range(0, GridHeight);
                    if (gridSystem.TryPlaceGem(width, height, new GridCoordinate(rx, ry)) != null)
                        placed = true;
                    attempts++;
                }
            }
        }
    }
}
