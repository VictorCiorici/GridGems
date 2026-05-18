using System;
using GridGame.Domain;
using GridGame.Controller;

namespace GridGame.Application
{
    /// <summary>
    /// Use case: start a new game session.
    /// Creates the domain grid, populates it via a generator, and transitions
    /// the <see cref="GameStateManager"/> to <see cref="GameState.Playing"/>.
    /// </summary>
    public class StartNewGameUseCase
    {
        private readonly GameStateManager _stateManager;
        private readonly IWinCondition _winCondition;
        private readonly IIdGenerator _idGenerator;

        /// <summary>
        /// Initializes a new <see cref="StartNewGameUseCase"/>.
        /// </summary>
        public StartNewGameUseCase(GameStateManager stateManager, IWinCondition winCondition, IIdGenerator idGenerator)
        {
            _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
            _winCondition = winCondition  ?? throw new ArgumentNullException(nameof(winCondition));
            _idGenerator  = idGenerator   ?? throw new ArgumentNullException(nameof(idGenerator));
        }

        /// <summary>
        /// Creates and populates a new grid, then starts the game session.
        /// </summary>
        /// <param name="generator">The strategy that determines grid size and gem placement.</param>
        /// <returns>The fully populated <see cref="GridSystem"/> ready for play.</returns>
        public GridSystem Execute(ILevelGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            var grid = new GridSystem(generator.GridWidth, generator.GridHeight, _idGenerator);
            generator.Populate(grid);
            _stateManager.StartGame(grid, _winCondition);
            return grid;
        }
    }
}
