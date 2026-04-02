# Development Log

## Commit 1 - Initial core models and documentation

### Summary
This commit establishes the initial project foundation for the MineSweeper product.

### Completed
- Created the first set of core models:
  - `Cell`
  - `Board`
  - `GameState`
  - `DifficultyLevel`
- Added XML documentation comments in English for maintainability and code clarity
- Added the first version of project documentation:
  - `README.md`
  - `docs/architecture.md`
  - `docs/development-log.md`

### Notes
At this stage, the project focuses on establishing a clean structure rather than gameplay features.
The next steps will introduce actual game behavior and automated tests in small, traceable commits.

## Commit 2 - Board initialization tests

### Summary
Introduced the first set of unit tests to validate the Board model.

### Completed
- Added unit tests for:
  - Board dimensions
  - Cell initialization
  - Cell coordinates
  - Default cell state

### Notes
This commit establishes a testing baseline for the project.
Future gameplay features will be developed alongside corresponding tests.

## Commit 3 - Initial game service

### Summary
Introduced the first version of the game service responsible for starting a new MineSweeper session.

### Completed
- Added `MineSweeperGame` as the central service for game lifecycle management
- Implemented `StartNewGame(rows, columns, mineCount)`
- Added unit tests for:
  - board creation
  - board configuration
  - initial game state transition
  - board replacement when starting a new game again

### Notes
This commit intentionally keeps the game service simple.
Mine placement and gameplay rules will be introduced in the next commits.

## Commit 4 - Mine placement logic

### Summary
Added random mine placement to the game service when starting a new MineSweeper session.

### Completed
- Extended `MineSweeperGame` to place mines during game initialization
- Ensured mine placement does not duplicate occupied cells
- Added unit tests to verify:
  - exact mine count
  - unique mine positions

### Notes
This commit focuses only on mine placement.
Adjacent mine calculation and reveal behavior will be introduced in later commits.

## Commit 5 - Documentation and game rules refinement

### Summary
Improved code readability and clarified core MineSweeper game rules.

### Completed
- Added Vietnamese inline comments to improve code understanding
- Enhanced XML documentation for maintainability
- Documented core game rules to guide future gameplay implementation

### Notes
This commit focuses on improving developer experience and ensuring that the rules of the game are clearly defined before implementing more complex gameplay features.

## Commit 6 - Adjacent mine calculation

### Summary
Implemented logic to calculate the number of adjacent mines for each cell.

### Completed
- Added method to calculate adjacent mine counts for all non-mine cells
- Handled edge and corner cases to prevent out-of-bounds errors
- Integrated calculation into game initialization flow
- Added deterministic unit tests with manual mine placement

### Notes
This step enables the core visual feedback of the MineSweeper game.
Each cell can now display the number of surrounding mines.

## Commit 7 - Cell reveal and flood fill

### Summary
Implemented cell reveal behavior including recursive flood fill and lose condition.

### Completed
- Added `RevealCell` method to handle user interaction
- Implemented recursive reveal (flood fill) for cells with zero adjacent mines
- Handled lose condition when revealing a mine (GameState = Lost)
- Prevented re-revealing already revealed cells
- Added unit tests for reveal and flood fill behavior

### Notes
This commit introduces the core interaction of the game.
Flood fill behavior ensures a smooth and intuitive gameplay experience.

## Commit 8 - Win condition checking

### Summary
Implemented logic to detect when the player has won the game.

### Completed
- Added `CheckWinCondition` method to verify if all non-mine cells are revealed
- Updated game state to `Won` when conditions are met
- Integrated win checking into the reveal flow
- Added unit tests for win scenarios

### Notes
This commit completes the game lifecycle by introducing the winning condition.
The game can now properly determine both success and failure states.

## Commit 9 - Flag toggle functionality

### Summary
Implemented flagging functionality to allow players to mark suspected mine locations.

### Completed
- Added `ToggleFlag` method to flag and unflag cells
- Prevented flagging of revealed cells
- Ensured flagging only works during active game state
- Added unit tests to verify flag behavior

### Notes
Flagging enhances gameplay by allowing players to track potential mine positions.
This feature completes the core interaction model of the MineSweeper game.

## Commit 10 - WPF MVVM structure and project reorganization

### Summary
Reorganized the WPF application structure and introduced a clean MVVM architecture for the UI layer.

### Completed
- Restructured project folders to follow MVVM principles:
  - Added `ViewModels/` for presentation logic
  - Added `Views/` for UI components
  - Added `Helpers/` for supporting utilities (future use)
  - Added `Assets/` for UI resources (reserved for future use)
- Moved `MainWindow.xaml` and code-behind into `Views/`
- Implemented `MainWindowViewModel` as the main UI controller
- Implemented `CellViewModel` to represent individual cells in the UI
- Connected View and ViewModel using WPF data binding via `DataContext`
- Established a clear separation between UI layer and core game logic

### Notes
This commit focuses on setting up a maintainable UI architecture rather than full gameplay interaction.
The next steps will integrate user interactions such as cell reveal and flagging into the MVVM structure.