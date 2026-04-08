# MineSweeper

A desktop MineSweeper game built with .NET and WPF.

---

## Project Overview

This project was developed as a product-oriented technical assessment.

The goal is not only to implement a fully functional MineSweeper game, but also to build a clean, maintainable, and user-friendly desktop application with a well-structured architecture and strong test coverage.

---

## Features

- Classic MineSweeper gameplay
- Multiple difficulty levels
- Flood fill (recursive reveal for empty cells)
- Flagging system
- Win / lose detection
- Visual indicators:
  - Exploded mine
  - Incorrect flags
- Responsive UI with MVVM pattern
- Unit test coverage for core logic and UI behavior
- Persistent-ready design (extensible for saving state, best score, etc.)

---

## Solution Structure

- `MineSweeper.App`  
  WPF desktop application responsible for UI rendering, user interaction, and ViewModels.

- `MineSweeper.Core`  
  Core domain models and game logic (UI-independent).

- `MineSweeper.Tests`  
  Unit tests for core logic, ViewModels, and helper components.

- `docs/`  
  Project documentation including architecture, development log, and game rules.

---

## Technology Stack

- .NET  
- WPF  
- MVVM pattern  
- xUnit  

---

## Code Documentation

The codebase uses **bilingual XML documentation (English / Vietnamese)**.

- English is used as the primary language for technical clarity and tooling support  
- Vietnamese is provided alongside to improve accessibility for local developers  

This applies consistently across:
- Core models  
- ViewModels  
- Helpers and services  
- Unit tests  

This approach makes the project:
- Easy to understand for international developers  
- More accessible for Vietnamese-speaking developers  
- Consistent and well-documented throughout  

---

## Testing

The project includes automated unit tests:

- Core domain logic is tested independently of UI  
- ViewModels are tested for UI behavior and state transitions  
- Helper components (Converters, Commands) are fully covered  

This ensures:
- Reduced regression risk  
- Confidence when refactoring  
- Stable gameplay behavior  

---

## Documentation

Detailed documentation is available in the `docs` folder:

- `docs/architecture.md`
- `docs/development-log.md`
- `docs/game-rules.en.md`
- `docs/game-rules.vi.md`
- `docs/usage-guide.md`
- `docs/release-notes.md`

---

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio
3. Set `MineSweeper.App` as startup project
4. Run the application

---

## Status

This project is completed and considered production-ready for its intended scope.

---

## Author

Developed as part of a technical and architectural learning journey focused on building clean, testable, and maintainable applications.