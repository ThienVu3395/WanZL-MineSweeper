# MineSweeper Game Rules

## 1. Objective

The objective of MineSweeper is to reveal all cells that do not contain mines without triggering any mine.

---

## 2. Game Board

- The game is played on a rectangular grid (board).
- Each cell can either:
  - Contain a mine
  - Be empty (safe)

---

## 3. Cell Properties

Each cell has the following properties:

- **IsMine**: Indicates whether the cell contains a mine
- **IsRevealed**: Indicates whether the cell has been revealed
- **IsFlagged**: Indicates whether the player marked the cell as a suspected mine
- **AdjacentMines**: Number of mines in the surrounding 8 neighboring cells

---

## 4. Game Initialization

At the start of the game:

1. A board is created with a fixed size (e.g., 9x9, 16x16)
2. A predefined number of mines are randomly placed
3. For each cell, the number of adjacent mines is calculated

---

## 5. Player Actions

### 5.1 Reveal a Cell (Left Click)

#### Case 1: Cell contains a mine
- The game ends immediately (Game Over)

#### Case 2: Cell does not contain a mine

- If **AdjacentMines > 0**:
  - Only that cell is revealed
  - The number is displayed

- If **AdjacentMines == 0**:
  - The cell is revealed
  - All neighboring cells are recursively revealed (flood fill)

---

### 5.2 Flag a Cell (Right Click)

- The player can mark a cell as a suspected mine
- Flagged cells cannot be revealed unless unflagged

---

## 6. Flood Fill Behavior

When a cell with **AdjacentMines == 0** is revealed:

- All neighboring cells are revealed automatically
- If a neighboring cell also has 0 adjacent mines, the process continues recursively
- If a neighboring cell has a number (> 0), it is revealed but does not expand further

---

## 7. Winning Condition

The player wins when:

> All non-mine cells are revealed

Flagging all mines is not required to win.

---

## 8. Losing Condition

The player loses when:

> A mine is revealed

---

## 9. Optional Rules (Advanced)

### 9.1 First Click Safety
- The first click is guaranteed to never hit a mine

### 9.2 Chording
- If a revealed cell has a number, and the number of flagged neighbors equals that number:
  - All remaining unflagged neighboring cells are revealed automatically

---

## 10. Summary

Core game logic consists of:

1. Board generation
2. Random mine placement
3. Adjacent mine calculation
4. Cell reveal logic with flood fill
5. Win/lose condition checking