import pygame
from board import Board, simplex_noise, random_noise, reseed
from drawer import draw_board, draw_heatmap, draw_directions, draw_clusters
from barricader import make_move, argmax_picker, retrace, generate_directions
from consts import *

# Width and height of pygame window
width, height = size = (800, 800)
numx, numy = (25, 25)

# Initialize pygame and create window application
pygame.init()

screen = pygame.display.set_mode(size)
pygame.display.set_caption("Barricader")

# Function for creating a new board
def create_board(gen: str = "simplex"):
    reseed()
    generation_func = lambda x, y: simplex_noise(x, y, 0.55, 6.0, 2) if gen == "simplex" else lambda x, y: random_noise(x, y, 0.65)
    return Board(numx, numy, generation_func)

# Create a new board and draw the board
board = create_board()
draw_board(screen, size, board)

# The current 'scene' currently getting drawn to the screen
# with options "board", "heatmap", "directionmap" and "clusters"
show_scene = "board"

# Drawing loop
while not pygame.event.peek(eventtype=pygame.QUIT):
    
    for event in pygame.event.get():
        # Toggle heatmap showing
        if event.type == pygame.KEYDOWN and event.key == pygame.K_b:
            show_scene = "heatmap"
        elif event.type == pygame.KEYUP and event.key == pygame.K_b:
            if show_scene == "heatmap": show_scene = "board"

        # Toggle direction map showing
        if event.type == pygame.KEYDOWN and event.key == pygame.K_c:
            show_scene = "clusters"
        elif event.type == pygame.KEYUP and event.key == pygame.K_c:
            if show_scene == "clusters": show_scene = "board"

        # Toggle direction map showing
        if event.type == pygame.KEYDOWN and event.key == pygame.K_v:
            show_scene = "directionmap"
        elif event.type == pygame.KEYUP and event.key == pygame.K_v:
            if show_scene == "directionmap": show_scene = "board"
        
        # Make move on board
        if event.type == pygame.KEYUP and event.key == pygame.K_n:
            move = make_move(board, argmax_picker)
            if move == None: continue
            board.set_barricade(move)
        
        # Create new map
        if event.type == pygame.KEYUP and event.key == pygame.K_SPACE:
            board = create_board()
        
        # Set cell to empty on left mouse
        if event.type == pygame.MOUSEBUTTONDOWN and event.button == pygame.BUTTON_LEFT:
            mx, my = event.pos
            i, j = int(mx * numx / width), int(my * numy / height)
            board.cells[i][j] = EMPTY
        
        # Toggle cell on right mouse
        if event.type == pygame.MOUSEBUTTONDOWN and event.button == pygame.BUTTON_RIGHT:
            mx, my = event.pos
            i, j = int(mx * numx / width), int(my * numy / height)
            board.cells[i][j] = (board.cells[i][j] + 1) % MAX_CELL

    # Clear all other events
    pygame.event.clear()

    # Display the correct scene
    if show_scene == "board": draw_board(screen, size, board)
    elif show_scene == "heatmap": draw_heatmap(screen, size, board)
    elif show_scene == "directionmap": draw_directions(screen, size, board)
    elif show_scene == "clusters": draw_clusters(screen, size, board)

    # If the current scene is board, draw the path from the mouse position to the flag
    if show_scene == "board":
        mx, my = pygame.mouse.get_pos()
        sx, sy = width / numx, height / numy
        coord = int(mx / sx), int(my / sy)

        pad = sx * 0.2

        directions = generate_directions(board, board.flagpos)
        for (x, y) in retrace(directions, coord):
            pygame.draw.rect(screen, (51, 77, 102), (x * sx + pad, y * sy + pad, sx - pad * 2, sy - pad * 2))

    # Render everything
    pygame.display.flip()

# Terminate pygame
pygame.quit()
