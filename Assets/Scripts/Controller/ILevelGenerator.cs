using GridGame.Domain;

namespace GridGame.Controller
{
    /// <summary>
    /// Defines a strategy for populating a GridSystem with gems.
    /// Implement this to create different level generation modes (procedural, predefined, tutorial, etc.).
    /// </summary>
    public interface ILevelGenerator
    {
        /// <summary>
        /// The width of the grid this generator produces.
        /// </summary>
        int GridWidth { get; }

        /// <summary>
        /// The height of the grid this generator produces.
        /// </summary>
        int GridHeight { get; }

        /// <summary>
        /// Populates the given grid system with gems.
        /// </summary>
        /// <param name="gridSystem">The grid to populate.</param>
        void Populate(GridSystem gridSystem);
    }
}
