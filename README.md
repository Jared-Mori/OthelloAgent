# Othello Game with AI Agent

A classic implementation of the board game Othello (also known as Reversi) featuring a fully functional AI agent. The game is built in [Unity/Python/etc.—replace with your tech stack], with clean code architecture and modular components for game logic, state management, and user interface.

## Features
- Standard Othello rules (8x8 board)
- Clickable UI with move highlights
- Player vs Player
- 2 difficulties of AI
- Play vs either of the AI's, or have the AI's play against one another.
- Weak Ai uses a simple evaluation function, Strong AI also utilizes a minimax function.

---

### Download & Play

1. Download build.zip
2. unzip and run Othello.exe

## Script Rundown
The scripts folder holds the driving code for the project, the rest is Unity related data.
A breakdown of the scripts that run the game are:

1. AIAgent
   Takes in gamestate data and runs both evaluation function and minimax function.
   
3. Disc
   Basic script that handles animating the disc objects
   
5. GameManager
   Singleton script that manages starting gamestates, settings,
   turn selection for player and AI, spawning and despawning disc objects, and displaying relevant UI elements.
   
7. GameState
   Handles gamelogic as well as evaluating valid moves. Also handles turn management and move making.
   
9. Highlight
    Script for valid move highlights
   
11. Player
    Basic Player Enum that defines teams. PlayerExtensions also handles some basic functions to return the opponent.
    
13. Position
    Pisition class that defines a position on the board, with some basic overrides for equation.
    
15. UIManager
    Manages UI animations and texts in unity.
