# Grid Gems

A 2D grid-based puzzle game developed in Unity 6, focusing on uncovering hidden gems by clicking on cells, inspired by classic mechanics like Minesweeper. 

## How to Launch the Game

1. **Open Unity:** Open the project folder (`Grid Game Assignment`) using **Unity 6 (6000.4.0f1)**.
2. **Open the Main Menu:** In the Unity Project window, navigate to `Assets/Scenes/` and double-click the **`main.unity`** scene.
   - *Note: You can also launch directly from the **`Playground.unity`** scene. The game will automatically detect the standalone launch and fallback to a purely procedural infinite mode.*
3. **Play:** Press the **Play** button at the top of the Unity Editor.
4. **Controls:**
   - Use the **Left Mouse Button** to click and reveal covered grid cells.
   - Use the UI buttons to cycle difficulties, launch the campaign, or play a procedurally generated level.

---

## What Has Been Done

This project was built from the ground up prioritizing enterprise-level code quality, separation of concerns, and robust architectural principles.

### Key Implementations:
- **Clean Architecture:** Strict separation between the **Domain** (pure C# game logic), **Application** (use cases and rules), and **Presentation/Controller** (Unity specific MonoBehaviours). The Domain layer has zero dependencies on the `UnityEngine` namespace.
- **Physical Assembly Boundaries (.asmdef):** Separated the codebase into 5 distinct Assembly Definitions (`GridGame.Domain`, `GridGame.Application`, `GridGame.Config`, `GridGame.Presentation`, and `GridGame.Controller`). This physically enforces clean dependencies at compiler level, preventing accidental cross-layer imports.
- **Persistence Abstraction:** Fully decoupled the `Application` layer from Unity Engine's static `PlayerPrefs` utility by introducing an `IPersistenceService` interface. The concrete `PlayerPrefsPersistenceService` resides in the outer Controller layer and is dynamically injected through the composition roots (`GamePresenter` and `MainMenuController`), opening the door for unit testing outside the Unity engine.
- **Dynamic Campaign Generator & Progression:** Custom Unity Editor tooling (`CampaignGenerator.cs`) capable of procedurally generating a 500-level Campaign with a smooth difficulty curve scaling grid size and gem counts. The game integrates seamlessly with the persistence abstraction to automatically save campaign progress.
- **Visual Policy Separation:** Decoupled the `GemCollection` data registry from its sizing, swapped-dimension check (handling 90° rotation), and fallback policies by introducing a dedicated `GemSpriteResolver` implementing `IGemSpriteResolver`.
- **Immutable Asset Encapsulation:** Protected `CampaignData.Levels` and `LevelData.Gems` as strict, read-only `IReadOnlyList<T>` properties. This prevents runtime scripts from polluting or mutating shared global configurations while preserving full setup mutability for custom Editor scripts via custom setup methods.
- **Design & Runtime Safeguards:** Implemented validation policies inside `LevelData.OnValidate()` to catch gem placement errors (overlaps or out-of-grid coordinates) inside the Unity Inspector at design-time, and added failure assertions in `PredefinedLevelGenerator.Populate()` that log precise runtime errors if placement fails.
- **Data-Driven Difficulty System:** The penalty/lives system is decoupled from code into a flexible `DifficultySettings` ScriptableObject. Designers can add unlimited new difficulty variants without programmer intervention. 
   - *How Difficulty is Calculated:* The game dynamically calculates the number of "empty cells" on the board (Total Grid Cells minus Cells occupied by Gems). It then multiplies this empty cell count by a percentage defined in the `DifficultySettings` (e.g., Normal = 35%, Brutal = 5%) to determine how many mistakes the player is allowed to make. It always guarantees a minimum of 1 life, and allows infinite lives on the "Peaceful" setting.
- **Event-Driven UI:** Zero `Update()` loops exist in the codebase. All UI state changes, visual grid updates, and condition checks are driven entirely by events (`OnStateChanged`, `OnMistakeMade`, `OnGemFound`). This results in maximum performance and zero idle CPU overhead.

---

## Development Focus

The primary focus of this assignment was **Architectural Robustness** and **Clean Code Practices**:
1. **Zero Global Singletons:** Bypassed the common Unity anti-pattern of global `Instance` singletons. Services are instantiated explicitly via a Composition Root (`GamePresenter.Awake`) and Dependency Injected.
2. **State-Based Thinking:** The UI is purely responsive. The Application layer's `GameStateManager` is the single source of truth, dictating if the game is `Idle`, `Playing`, `Won`, or `Lost`. The Presentation layer simply reads immutable `GameProgress` snapshots.
3. **Scalability & Extensibility:** The use of interfaces (`IWinCondition`, `ILevelGenerator`, `IDifficultyConfig`) makes it trivial to introduce new game modes (e.g., Timed Mode) or generator rules without touching existing core logic.
4. **No Third-Party Bloat (Pure DI & Native Events):** We intentionally avoided heavy frameworks like **Zenject/Extenject** and **UniRx**. 
   - *Instead of Zenject:* We use **Pure Dependency Injection** via a Composition Root (`GamePresenter`). This avoids reflection-based startup overhead, prevents black-box magic, and keeps the architecture lean.
   - *Instead of UniRx:* We use native C# `event Action` delegates. They are perfectly sufficient for our reactive, event-driven UI, and avoiding UniRx prevents unnecessary GC allocations, steep learning curves, and dependency on external packages breaking in future Unity versions.
5. **Why Not ECS? (Entity Component System):** We intentionally avoided Unity's DOTS/ECS framework. While ECS provides incredible performance for games with tens of thousands of moving entities, it is severe overengineering for a static, grid-based puzzle game. ECS systems iterate over contiguous data arrays every single frame. Instead, our Event-Driven architecture executes zero logic until the player clicks a cell, which is vastly more CPU-efficient for this specific genre and avoids the immense boilerplate and development overhead associated with DOTS.

---

## What is Missing for a Full-Fledged Game?

While the core mechanics and architecture are production-ready, the project currently exists as a "Minimum Viable Product" (MVP). To become a fully polished, commercial-ready game, the following features would be required:

### 1. Visual Polish & "Juice"
- **Particle Systems:** Dust explosions when a cell is revealed, and sparkling effects when a gem is fully discovered.
- **Micro-Animations:** UI tweening (using DOTween or similar) for menus popping in, buttons scaling on hover, and smooth transitions between the Main Menu and Playground.
- **Screen Shake:** Subtle feedback when a mistake is made.

### 2. Audio Design
- Implement an `IAudioService` wrapper around Unity's AudioSource.
- Add sound effects for interacting with the UI, breaking dirt, hitting a gem, winning, and losing.
- Add ambient background music.

### 3. Gameplay Depth
- **Power-ups & Items:** Tools like a "radar" or "bomb" that allow the player to scan or clear multiple cells at once.
- **Meta-Progression:** A currency system (earned by finding gems) to buy said power-ups or unlock cosmetics.
- **Timed Modes:** Implementing a time-limit based `IWinCondition` or `ILoseCondition`.

### 4. Production Systems
- **Cloud Save / Auth:** Migrating from `PlayerPrefs` to a remote backend (like Unity Gaming Services or Firebase) to save progress across devices.
- **Analytics & Crashlytics:** Implementing telemetry to track player drop-off rates and difficulty bottlenecks.
- **Localization:** A text management system to support multiple languages for the UI.
- **Mobile Support:** Adjusting Canvas scaling for notch support, handling application pause/resume states, and ensuring input correctly tracks touch gestures.
