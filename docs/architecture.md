# Architecture

## Overview

The solution is intentionally split into separate projects to keep responsibilities clear,
improve maintainability, and support testability.

## Project Structure

### MineSweeper.App
This project contains the WPF desktop application.
Its responsibility is to render the user interface, handle user interaction,
and communicate with the core game logic.

### MineSweeper.Core
This project contains the core domain models and will contain the main game rules.
It is designed to be UI-independent so that gameplay behavior can be tested in isolation.

### MineSweeper.Tests
This project contains automated tests for the core logic.
The goal is to validate game rules and prevent regressions as the product evolves.

## Design Direction

The current design follows a simple separation of concerns:

- UI logic stays in the WPF application layer
- Game rules and models stay in the core layer
- Verification of behavior stays in the test layer

This structure keeps the product easier to extend and maintain over time.

## Initial Core Models

The first iteration introduces the following foundational models:

- `Cell`: represents a single tile on the board
- `Board`: represents the full game board and its dimensions
- `GameState`: represents the state of the game session
- `DifficultyLevel`: represents the supported difficulty presets

These models provide the foundation for future gameplay mechanics such as
mine placement, reveal behavior, flagging, win/loss detection, and custom game settings.