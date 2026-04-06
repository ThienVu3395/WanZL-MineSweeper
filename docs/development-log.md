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

## Commit 11 - WPF cell interaction with MVVM command binding

### Summary
Enabled user interaction in the WPF UI by connecting cell click actions to the core game logic using MVVM command binding.

### Completed
- Added `RelayCommand` to support command-based interaction in MVVM
- Implemented `RevealCellCommand` in `MainWindowViewModel`
- Handled cell click events and mapped them to the `RevealCell` logic in the game service
- Passed `CellViewModel` as command parameter from the UI
- Implemented UI refresh mechanism via `CellViewModel.Refresh()`
- Updated game status dynamically after each interaction
- Fixed application startup issue after restructuring by updating `StartupUri` in `App.xaml`

### Notes
This commit completes the core interaction loop between the UI and the game logic.
The game is now fully playable through the WPF interface.

The next step will focus on implementing right-click flag functionality to align with standard MineSweeper behavior.

## Commit 12 - Flag toggle functionality

### Summary
Implemented right-click flag functionality to mark suspected mine cells.

### Completed
- Added `ToggleFlag` method in game service
- Prevented revealing flagged cells
- Implemented `ToggleFlagCommand` in ViewModel
- Handled right-click interaction in WPF UI
- Updated UI to display flag indicator (🚩)
- Ensured UI refresh after flag toggle

### Notes
This commit completes the core MineSweeper gameplay loop.
The application now supports both revealing and flagging cells, aligning with standard game behavior.

## Commit 13 - New game control and difficulty selection

### Summary
Added user controls to start a new game and select difficulty directly from the WPF interface.

### Completed
- Added difficulty selection using the existing `DifficultyLevel` enum
- Exposed available difficulty options in `MainWindowViewModel`
- Added `SelectedDifficulty` property for UI binding
- Added `NewGameCommand` to start a new game based on the selected difficulty
- Mapped difficulty levels to standard board configurations
- Updated the WPF UI to include a difficulty dropdown and a new game button
- Added scroll support for larger boards

### Notes
This commit improves usability by allowing the player to restart the game and switch difficulty without restarting the application.
It also makes the product feel more complete and closer to a real desktop game.

## Commit 14 - WPF UI polish and endgame interaction lock

### Summary
Improved the WPF user interface with clearer visual states, friendlier endgame feedback, and disabled interaction after the game is finished.

### Completed
- Improved visual styling for hidden, revealed, flagged, and mined cells
- Replaced text-based mine and flag symbols with emoji icons (`💣`, `🚩`)
- Added hover feedback and clearer cell state presentation
- Enhanced game status text with user-friendly win and lose messages
- Added popup notifications for win and lose conditions
- Disabled further reveal and flag interactions after the game ends using command state control

### Notes
This commit focuses on usability and presentation quality.
The game now feels more polished and communicates the endgame state more clearly to the player.

## Commit 15 - Mine counter and flag tracking

### Summary
Added live mine and flag counters to improve gameplay awareness and bring the interface closer to a classic MineSweeper experience.

### Completed
- Added total mine count display
- Added current flag count display
- Added remaining mine estimate based on placed flags
- Updated the UI to refresh counter values during gameplay
- Integrated counter updates into new game and flag interactions

### Notes
This commit improves gameplay clarity by giving the player immediate feedback about board progress and flag usage.
It also makes the desktop application feel closer to a complete game product.

## Commit 16 - Colored numbers for revealed cells

### Summary
Enhanced the visual clarity of the game by applying color coding to numbers representing adjacent mines.

### Completed
- Added color styling for numbers 1 to 8 based on classic MineSweeper conventions
- Applied conditional UI triggers based on cell state and adjacent mine count
- Improved readability and visual distinction between different cell values

### Notes
This change aligns the UI with traditional MineSweeper design,
making the game more intuitive and visually recognizable.

## Commit 17 - Add toast notification for flag limit warning

### Summary
Implemented a basic toast notification system to display warning messages (e.g., when user exceeds flag limit).

### Details
- Added `Message` property in `MainWindowViewModel` for temporary UI messages
- Displayed message below game status in the UI
- Implemented `ShowFlagLimitMessage()` to notify when all flags are used
- Integrated toast animation in `MainWindow.xaml.cs` using fade in/out effect
- Toast is triggered when user attempts to place more flags than allowed

### Notes
- Current implementation does not handle animation interruption or message clearing properly
- Known issue: toast may remain visible with empty content when message is cleared
- This will be addressed in a separate commit (toast state synchronization improvement)

## Commit 18: Resolve toast visibility bug when message is cleared

### Summary
Fixed an issue where the toast notification remained visible without content when the message was cleared.

### Details
- Cleared `Message` property when:
  - Flag is toggled successfully (unflag or valid flag)
  - A cell is revealed
  - A new game is started
- Updated toast display logic to hide the toast when message is null or empty
- Prevented animation from running when there is no message content

### Root Cause
Toast animation was still running even after `Message` was cleared, resulting in an empty visible toast.

## Commit 19 & 19+: add unit tests for ViewModels (MainWindowViewModel, CellViewModel) and Helpers (NullToVisibilityConverter, RelayCommand)

### Summary
Added test suite for the MVVM application layer.

### Details
- Verified default game initialization and difficulty switching
- Tested flag toggle behavior, flag limits, and remaining mine count updates
- Verified that warning messages are shown and cleared correctly
- Tested command enable/disable behavior when the game finishes
- Verified PropertyChanged notifications for important ViewModel properties
- Added CellViewModel tests for display text mapping and model synchronization

- Tested NullToVisibilityConverter for null, empty, and non-empty values
- Verified RelayCommand execution, CanExecute behavior, and event triggering

### Notes
These tests focus on behavior and state transitions in the ViewModel layer rather than WPF rendering.
The goal is to improve confidence in MVVM correctness and reduce regression risk as new features are added.

## Commit 20 - Difficulty description display

### Summary
Improved the difficulty selection experience by showing board size and mine count information in the UI.

### Completed
- Added formatted difficulty descriptions for all supported game modes
- Displayed board dimensions and mine count directly in the difficulty selector
- Improved clarity so players can understand each difficulty before starting a new game

### Notes
This commit focuses on usability rather than core gameplay logic.
The difficulty selector now provides more context to the player and makes the interface feel more complete.

## Commit 21 - Recursive reveal safety improvement

### Summary
Improved recursive reveal behavior to ensure flagged cells are not automatically revealed during flood fill.

### Completed
- Updated recursive reveal logic to skip flagged cells
- Preserved existing flood fill behavior for empty regions
- Improved consistency with standard MineSweeper gameplay rules

### Notes
Previously, flagged cells could be unintentionally revealed during recursive expansion.
This update ensures that player-marked cells are respected by the flood fill algorithm.

## Commit 22 - Reveal all mines on lose

### Summary
Improved lose-state behavior by revealing all mine cells when the player hits a mine.

### Completed
- Added logic to reveal all mines after a losing move
- Preserved existing lose condition handling
- Added unit test to verify that all mines become visible when the game ends in loss

### Notes
This change improves gameplay feedback and aligns the lose-state behavior more closely with standard MineSweeper implementations.

## Commit 23 - Centralize board neighbor traversal in game service

### Summary
Refactored neighbor traversal logic in the game service to reduce duplication and prepare for future gameplay features.

### Completed
- Extracted shared neighbor traversal logic into a reusable helper method (`GetNeighborCells`)
- Reused the helper in adjacent mine calculation
- Reused the helper in recursive reveal logic
- Improved code readability and reduced duplicated loops

### Notes
This is a non-functional refactor (no behavior change).
It prepares the codebase for upcoming features such as chording,
which relies heavily on neighbor cell traversal.

## Commit 24 - Implement chording gameplay with unit test coverage

### Summary
Added the chording mechanic to the core MineSweeper gameplay and covered it with dedicated unit tests.

### Completed
- Added `ChordCell(row, column)` to `MineSweeperGame`
- Enabled chording only for revealed cells during an active game
- Counted flagged neighboring cells and compared them against the revealed cell's adjacent mine count
- Revealed all hidden and unflagged neighboring cells when the chording condition was satisfied
- Reused existing reveal logic to preserve flood fill, lose-state, and win-state consistency
- Stopped chord processing immediately if an unflagged mine was revealed
- Added unit tests for key chording scenarios:
  - target cell is not revealed
  - adjacent flag count does not match
  - valid chord reveals hidden unflagged neighbors
  - incorrect flags can trigger a loss
  - valid chord can complete the game and trigger a win

### Notes
This commit introduces an important advanced MineSweeper interaction found in classic implementations.
It improves gameplay efficiency for experienced players while keeping the logic consistent with the existing reveal and win/loss rules.

## Commit 25 - Add UI interaction for chording on revealed cells

### Summary
Connected the chording gameplay mechanic to the WPF user interface so players can trigger it directly during gameplay.

### Completed
- Added `ChordCellCommand` to `MainWindowViewModel`
- Implemented UI handling for chording through double-click interaction on board cells
- Forwarded double-click events from the view to the ViewModel in a way consistent with the existing right-click flag flow
- Refreshed board state, counters, and game status after chord actions
- Kept endgame feedback consistent with existing reveal behavior
- Added a small gameplay hint to make the new interaction easier to discover

### Notes
This commit completes the first end-to-end version of the chording feature.
Players can now use the mechanic directly from the desktop UI instead of only through core logic and tests.

## Commit 26 - Add MainWindowViewModel coverage for chording command

### Summary
Extended the ViewModel test suite to cover the new chording interaction exposed to the WPF UI.

### Completed
- Added unit tests for `ChordCellCommand` in `MainWindowViewModel`
- Verified that chording is only executable for valid revealed cells during active gameplay
- Verified that chord execution reveals neighboring cells when the rule conditions are satisfied
- Verified that ViewModel state and board-related properties are updated after chord actions
- Verified that the chord command is disabled when the game is already finished

### Notes
This commit improves regression safety for the chording feature at the MVVM layer.
It helps ensure that the WPF interaction flow stays aligned with the underlying core gameplay logic.

## Commit 27 - Simplify board refresh and UI state update flow

### Summary
Refactored `MainWindowViewModel` to reduce duplicated UI refresh and post-action update logic after gameplay interactions.

### Completed
- Extracted shared helper methods for:
  - clearing temporary messages
  - refreshing board state
  - raising property change notifications
  - refreshing command executable states
  - handling endgame notifications
- Simplified `OnRevealCell`, `OnToggleFlag`, and `OnChordCell` by reusing shared post-action helper methods
- Simplified `StartNewGame` by reusing shared refresh-related helpers
- Improved readability and consistency of interaction handling in `MainWindowViewModel`
- Preserved existing gameplay behavior and ViewModel behavior

### Notes
This commit is a maintainability-focused refactor and does not introduce new gameplay features.
It prepares the ViewModel for future enhancements by making action handling more consistent, reusable, and easier to extend.

## Commit 28 - Extend difficulty extensions with board preset mapping

### Summary
Refactored difficulty configuration by extending `DifficultyExtensions` to provide both display text and reusable board preset mapping.

### Completed
- Extended `DifficultyExtensions` with `ToPreset()` to convert `DifficultyLevel` into board configuration data
- Added nested `DifficultyPreset` model inside `DifficultyExtensions` to represent rows, columns, and mine count
- Updated `MainWindowViewModel` to start new games using reusable preset mapping instead of inline hardcoded switch values
- Preserved existing behavior for Beginner, Intermediate, and Expert difficulties
- Added unit tests for:
  - difficulty display text mapping
  - difficulty preset mapping
  - fallback behavior for unsupported difficulty values

### Notes
This commit improves maintainability by centralizing difficulty-related display and configuration logic in one place.
It also prepares the project for future enhancements such as custom difficulty support and reusable preset-based setup.

## Commit 29 - Move endgame notifications out of main window view model

### Summary
Refactored endgame notification handling so that popup display is no longer controlled directly by `MainWindowViewModel`.

### Completed
- Added a dedicated view model event to notify when the game ends
- Removed direct `MessageBox` usage from `MainWindowViewModel`
- Updated the WPF view to listen for endgame notifications and display the appropriate popup
- Preserved existing win and lose feedback behavior for the player
- Added ViewModel test coverage for the new endgame notification event

### Notes
This commit improves MVVM separation by keeping UI-specific dialog behavior inside the view layer instead of the view model.
It also makes the view model easier to maintain and test.

## Commit 30 - Ensure first click is always safe

### Summary
Improved the core gameplay experience by guaranteeing that the first revealed cell is always safe.

### Completed
- Updated game initialization flow so mine placement is deferred until the first reveal action
- Ensured that the first revealed cell can never contain a mine
- Recalculated adjacent mine counts after first-click mine placement
- Preserved the configured total mine count after first-click setup
- Added unit tests to verify:
  - the first reveal never causes an immediate loss
  - the first revealed cell is always safe
  - the configured mine count is preserved
  - later reveals can still trigger a normal loss condition

### Notes
This commit improves user experience significantly by removing unfair first-click losses.
It keeps the overall gameplay rules intact while making the product feel more polished and player-friendly.

## Commit 31 - Validate board configuration before starting a new game

### Summary
Improved game initialization safety by validating board configuration before creating a new MineSweeper session.

### Completed
- Added validation for invalid board dimensions
- Added validation for negative mine counts
- Added validation to prevent mine counts that would make a safe first reveal impossible
- Ensured invalid configurations fail fast with clear exceptions instead of causing undefined behavior or infinite loops
- Added unit tests for invalid and valid board configuration scenarios

### Notes
This commit strengthens reliability after the first-click-safe feature introduced stricter constraints on valid board setup.
It prevents impossible or unsafe configurations from reaching the gameplay loop.

## Commit 32 - Add game timer and elapsed duration display

### Summary
Added a gameplay timer to track elapsed duration and display it directly in the WPF interface.

### Completed
- Added timer state and elapsed time properties to `MainWindowViewModel`
- Started the timer on the first valid reveal action
- Stopped the timer automatically when the game ended
- Reset the timer when starting a new game
- Displayed elapsed time in the main game header alongside existing counters
- Added ViewModel unit tests for:
  - timer initialization
  - timer start on first reveal
  - timer reset on new game
  - elapsed time formatting
  - timer stop on game over

### Notes
This commit improves product value by making the game feel more complete and closer to a classic MineSweeper experience.
It also prepares the project for future features such as best-time tracking by difficulty.