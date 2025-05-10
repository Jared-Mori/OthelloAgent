import pygame
import sys
import numpy as np
import time

# Initialize Pygame
pygame.init()

# Constants
SIZE = 600
ROWS, COLS = 8, 8
CELL_SIZE = SIZE // ROWS
BLACK = (0, 0, 0)
WHITE = (255, 255, 255)
GREEN = (34, 139, 34)
GRAY = (169, 169, 169)

# Board representation
EMPTY = 0
BLACK_DISC = 1
WHITE_DISC = 2

# Initialize board
board = np.zeros((ROWS, COLS), dtype=int)
board[3, 3] = WHITE_DISC
board[3, 4] = BLACK_DISC
board[4, 3] = BLACK_DISC
board[4, 4] = WHITE_DISC

# Pygame setup
screen = pygame.display.set_mode((SIZE, SIZE))
pygame.display.set_caption("Othello with AI")
font = pygame.font.SysFont(None, 36)

# Directions for checking flips
DIRECTIONS = [(-1, -1), (-1, 0), (-1, 1),
              (0, -1),          (0, 1),
              (1, -1),  (1, 0), (1, 1)]

current_player = BLACK_DISC  # Human starts


def draw_board():
    screen.fill(GREEN)
    for row in range(ROWS):
        for col in range(COLS):
            rect = pygame.Rect(col * CELL_SIZE, row * CELL_SIZE, CELL_SIZE, CELL_SIZE)
            pygame.draw.rect(screen, BLACK, rect, 1)
            if board[row, col] == BLACK_DISC:
                pygame.draw.circle(screen, BLACK, rect.center, CELL_SIZE // 2 - 5)
            elif board[row, col] == WHITE_DISC:
                pygame.draw.circle(screen, WHITE, rect.center, CELL_SIZE // 2 - 5)


def is_valid_move(row, col, player):
    if board[row, col] != EMPTY:
        return False
    opponent = WHITE_DISC if player == BLACK_DISC else BLACK_DISC
    for dr, dc in DIRECTIONS:
        r, c = row + dr, col + dc
        flipped = []
        while 0 <= r < ROWS and 0 <= c < COLS and board[r, c] == opponent:
            flipped.append((r, c))
            r += dr
            c += dc
        if flipped and 0 <= r < ROWS and 0 <= c < COLS and board[r, c] == player:
            return True
    return False


def apply_move(row, col, player):
    opponent = WHITE_DISC if player == BLACK_DISC else BLACK_DISC
    board[row, col] = player
    for dr, dc in DIRECTIONS:
        r, c = row + dr, col + dc
        flipped = []
        while 0 <= r < ROWS and 0 <= c < COLS and board[r, c] == opponent:
            flipped.append((r, c))
            r += dr
            c += dc
        if flipped and 0 <= r < ROWS and 0 <= c < COLS and board[r, c] == player:
            for fr, fc in flipped:
                board[fr, fc] = player


def apply_move_temp(temp_board, row, col, player):
    opponent = WHITE_DISC if player == BLACK_DISC else BLACK_DISC
    temp_board[row, col] = player
    for dr, dc in DIRECTIONS:
        r, c = row + dr, col + dc
        flipped = []
        while 0 <= r < ROWS and 0 <= c < COLS and temp_board[r, c] == opponent:
            flipped.append((r, c))
            r += dr
            c += dc
        if flipped and 0 <= r < ROWS and 0 <= c < COLS and temp_board[r, c] == player:
            for fr, fc in flipped:
                temp_board[fr, fc] = player


def ai_best_move(player):
    valid_moves = get_valid_moves(player)
    best_score = -1
    best_move = None
    for row, col in valid_moves:
        temp_board = board.copy()
        apply_move_temp(temp_board, row, col, player)
        score = np.count_nonzero(temp_board == player)
        if score > best_score:
            best_score = score
            best_move = (row, col)
    return best_move


def get_valid_moves(player):
    return [(r, c) for r in range(ROWS) for c in range(COLS) if is_valid_move(r, c, player)]


def count_discs():
    blacks = np.count_nonzero(board == BLACK_DISC)
    whites = np.count_nonzero(board == WHITE_DISC)
    return blacks, whites


def draw_winner():
    blacks, whites = count_discs()
    if blacks > whites:
        result = "Black wins!"
    elif whites > blacks:
        result = "White wins!"
    else:
        result = "It's a tie!"
    text = f"Game Over! {result} (Black: {blacks}, White: {whites})"
    label = font.render(text, True, GRAY)
    screen.blit(label, (10, SIZE - 40))


# Game loop
running = True
game_over = False
while running:
    draw_board()
    if not game_over:
        valid_moves = get_valid_moves(current_player)
        if not valid_moves:
            other_player = WHITE_DISC if current_player == BLACK_DISC else BLACK_DISC
            if not get_valid_moves(other_player):
                game_over = True
            else:
                current_player = other_player

    if not game_over and current_player == WHITE_DISC:
        pygame.display.flip()
        time.sleep(0.5)  # Pause to see AI move
        move = ai_best_move(WHITE_DISC)
        if move:
            apply_move(move[0], move[1], WHITE_DISC)
            current_player = BLACK_DISC

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

        elif event.type == pygame.MOUSEBUTTONDOWN and current_player == BLACK_DISC and not game_over:
            x, y = event.pos
            row, col = y // CELL_SIZE, x // CELL_SIZE
            if is_valid_move(row, col, BLACK_DISC):
                apply_move(row, col, BLACK_DISC)
                current_player = WHITE_DISC

    if game_over:
        draw_winner()

    pygame.display.flip()

pygame.quit()
sys.exit()
