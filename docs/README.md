# MineSweeper

A desktop MineSweeper game built with .NET 8 and WPF following the MVVM pattern.

---

## Project Overview

This project was developed as a **product-oriented technical assessment**.

The goal is to implement a fully functional MineSweeper game and deliver a clean, maintainable, and user-friendly desktop application with:

- Well-structured architecture (MVVM)
- Strong separation of concerns
- High unit-test coverage
- Clear and consistent documentation

---

## Features

- Classic MineSweeper gameplay
- Multiple difficulty levels (Beginner, Intermediate, Expert, Custom)
- Flood fill (recursive reveal for empty cells)
- Flagging system with limit validation
- Win / lose detection and end-of-game handling
- Visual indicators:
  - Exploded mine
  - Incorrect flags
- Game state management:
  - In-progress tracking
  - Win/Loss handling
- Persistent data:
  - Best time tracking
  - Player preferences
  - Game session restore
- Responsive UI using MVVM pattern
- Comprehensive unit test coverage

---

## Solution Structure

- `MineSweeper.App`  
  WPF desktop application (UI layer, Views, ViewModels, Commands)

- `MineSweeper.Core`  
  Core domain logic (Board, Cell, GameState, Rules)

- `MineSweeper.Tests`  
  Unit tests for game logic, ViewModel behavior and persistence/state transitions

- `docs/`  
  Project documentation: architecture, development log, game rules and usage guide

---

## Technology Stack

- .NET 8
- WPF
- MVVM pattern
- xUnit (unit testing)

---

## Code Documentation

The codebase uses **bilingual XML documentation (English / Vietnamese)**.

- English → primary for technical clarity and tooling
- Vietnamese → improves accessibility for local developers

Applied consistently across:
- Core logic
- ViewModels
- Helpers
- Unit tests

This ensures:
- Better readability
- Easier onboarding
- Consistent documentation style

---

## Testing

The project includes comprehensive automated tests:

- Core logic is tested independently of UI
- ViewModels are tested for:
  - State transitions
  - Command behavior
  - Event handling
- Persistence logic is tested for:
  - Save / restore session
  - Invalid data fallback
  - File consistency

### Run tests

Using .NET CLI: `dotnet test`

Or using Visual Studio: Open Test Explorer => Click Run All Tests

---

## Data Persistence

The application uses local JSON files for storing user data:
  - `best-times.json` — stores best completion times
  - `player-preferences.json` — stores selected difficulty and custom settings
  - `lastsession.json` — stores current in-progress session

Behavior:

- Session is automatically saved during gameplay
- Session is cleared when:
  - Player wins or loses
  - Player manually resets the game
  - New game or restart
- Best time is updated only when improved
- Player preferences persist across sessions
- Invalid or corrupted data is safely ignored

---

## Getting Started

Prerequisites

- .NET 8 SDK: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
- Visual Studio 2022/2026 or later with .NET desktop development workload (recommended) or a code editor + CLI

Clone the repository

```powershell
git clone https://github.com/ThienVu3395/WanZL-MineSweeper.git
cd WanZL-MineSweeper
```

Build and run from Visual Studio

1. Open the solution (`*.sln`) in Visual Studio.
2. Set `MineSweeper.App` as the startup project.
3. Build and Run (F5) or Run without debugging (Ctrl+F5).

Build and run from the command line

```powershell
# From repository root
dotnet build
dotnet run --project MineSweeper.App\MineSweeper.App.csproj
```

Run tests

```powershell
dotnet test
```

Configuration

- Difficulty and custom board sizes can be configured from the app UI.
- Preferences are saved automatically when changed.

---

## Usage

Controls

- Left click: Reveal a cell
- Right click: Toggle flag on a cell
- Middle click (or left+right): Reveal surrounding cells when number of surrounding flags equals cell number (if supported by UI)

Winning and losing

- Win: All non-mine cells revealed and mines flagged correctly
- Lose: Revealing a mine — the mine will be shown as exploded and the game ends

UI Indicators

- Exploded mine icon when losing
- Incorrect flags are shown after the game ends
- Timer and remaining flags counter visible on the main window

---

## Tests and Quality

- Unit tests (xUnit) cover core game rules, flood-fill behavior, ViewModel commands and state transitions, and persistence handling.
- Run `dotnet test` from solution root to execute the test suite.

---

## Publish (Optional)

You can publish the WPF application for Windows using `dotnet publish` or create an installer (recommended for distribution).

Self-contained publish (Windows x64)

```powershell
dotnet publish MineSweeper.App\MineSweeper.App.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=false
```

Produce a single-file executable (larger, but simpler distribution)

```powershell
dotnet publish MineSweeper.App\MineSweeper.App.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Installer options

- MSIX / MSIX Packaging in Visual Studio
- Third-party installer tooling (Inno Setup, WiX)

Notes

- When publishing, validate that asset files (icons, resources) are included in the publish profile.
- If using ClickOnce or MSIX, ensure the app manifest and permissions are set correctly for file-system persistence.

---

## Contributing

- Fork the repository and create feature branches for new work.
- Follow existing architecture and coding conventions.
- Add unit tests for new logic or bug fixes.
- Open pull requests with a description and link to any related issue.

---

## License

This project is provided as-is. Check the repository root for a `LICENSE` file to confirm licensing terms.

---

## Contact

For questions about the code, open an issue on the repository or contact the author via the project GitHub.