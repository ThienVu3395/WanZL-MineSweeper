# Architecture

## Overview

The solution is structured into multiple projects to ensure clear separation of concerns,
high maintainability, and strong testability.

It follows a layered architecture combined with the MVVM (Model-View-ViewModel) pattern
for the UI, allowing the application to remain modular, scalable, and easy to extend.

---

## Project Structure

### MineSweeper.App

This project contains the WPF desktop application.

**Responsibilities:**
- Render the user interface
- Handle user interactions
- Manage UI state via ViewModels
- Bind UI components to application logic
- Provide UI-specific helpers (Converters, Commands)

**Key Patterns:**
- MVVM (View + ViewModel + Binding)
- ICommand (RelayCommand) for user actions

---

### MineSweeper.Core

This project contains the core domain models and game logic.

**Responsibilities:**
- Represent the game state and domain entities
- Encapsulate core gameplay behavior
- Remain completely independent from UI frameworks

**Design Goals:**
- No dependency on WPF or UI concerns
- Easily testable logic
- Clear and simple domain modeling

---

### MineSweeper.Tests

This project contains automated unit tests.

**Responsibilities:**
- Verify correctness of core models and logic
- Validate ViewModel behavior
- Ensure UI-supporting components behave as expected
- Prevent regressions as the system evolves

---

## Architectural Patterns

### Layered Architecture

The solution follows a simplified layered structure:
	UI (WPF): UI interacts only with ViewModels
	↓
	ViewModel (App): ViewModels expose state and coordinate interactions
	↓
	Core (Domain): Core contains pure domain logic with no UI dependencies

---

### Why not full Clean Architecture?

Although the project follows many Clean Architecture principles, it does not strictly implement full Clean Architecture.

This is intentional:

- The project scope is relatively small (desktop game)
- Adding extra abstraction layers (Application, Infrastructure, etc.) would introduce unnecessary complexity
- The current structure already ensures:
  - Clear separation of concerns  
  - High testability  
  - Maintainability  

This approach keeps the codebase simple, readable, and practical while still following good architectural practices.

---

### MVVM (Model-View-ViewModel)

The UI layer follows the MVVM pattern:

- **View**: XAML UI (presentation only)  
- **ViewModel**: UI logic, state management, commands  
- **Model**: Core domain objects (`Cell`, `Board`, etc.)

**Benefits:**
- Clear separation between UI and logic  
- Easier unit testing  
- Better maintainability and scalability  

---

## Core Domain Models

The application is built around the following core models:

- `Cell`: represents a single tile on the board  
- `Board`: represents the full game grid and its structure  
- `GameState`: represents the current state of the game session  
- `DifficultyLevel`: represents predefined difficulty configurations  

These models provide the foundation for gameplay features such as:

- Mine placement  
- Reveal logic (including flood fill)  
- Flagging behavior  
- Win/loss detection  
- Custom difficulty support  

---

## Design Principles

The architecture is guided by the following principles:

- **Separation of Concerns**  
  Each layer has a clearly defined responsibility  

- **Single Responsibility Principle (SRP)**  
  Classes are designed with a single purpose  

- **Testability First**  
  Core logic and ViewModels are designed for unit testing  

- **UI Independence**  
  The domain layer does not depend on UI frameworks  

---

## Testing Strategy

The project emphasizes automated testing:

- Core models are tested independently of UI  
- ViewModels are tested for UI logic and state transitions  
- Helper components (Converters, Commands) are covered with focused unit tests  

**Goals:**
- Reduce regression risk  
- Ensure correctness of gameplay logic  
- Maintain confidence during refactoring  

---

## Evolution

The architecture was designed to evolve incrementally:

- **Initial phase**: basic models and structure  
- **Mid phase**: MVVM integration and UI behavior  
- **Final phase**: persistence, services, and full test coverage  

This approach balances learning clarity with production-level quality.