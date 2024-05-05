import pygame
from barricader import generate_heatmap, generate_directions, generate_clusters
from consts import *

# 0=empty, 1=blocked, 2=barricade, 3=flag
colors = [(49, 50, 56), (30, 30, 30), (109, 47, 54), (76, 119, 56)]

# Draw a board-state
def draw_board(screen, size, board):
    sx, sy = size[0] / board.width, size[1] / board.height

    for i, row in enumerate(board.cells):
        for j, cell in enumerate(row):

            col = colors[cell]
            pygame.draw.rect(screen, col, (i * sx, j * sy, sx, sy))

# Draw a heatmap
def draw_heatmap(screen, size, board):
    sx, sy = size[0] / board.width, size[1] / board.height
    heatmap = generate_heatmap(board)

    for i in range(board.width):
        for j in range(board.height):
            
            z = int(heatmap[i, j] * 255)
            col = (z, z, z)

            pygame.draw.rect(screen, col, (i * sx, j * sy, sx, sy))

# Draw a direction-map
def draw_directions(screen, size, board):
    sx, sy = size[0] / board.width, size[1] / board.height
    directions = generate_directions(board, board.flagpos)

    pad = sx * 0.2

    for i in range(board.width):
        for j in range(board.height):
                 
            if directions[i, j] == NODIR:
                pygame.draw.rect(screen, colors[BLOCKED], (i * sx, j * sy, sx, sy))
                continue
            
            pygame.draw.rect(screen, colors[EMPTY], (i * sx, j * sy, sx, sy))

            oppsite_dir = (directions[i, j] + 2) % 4
            midpoints = [(i * sx + sx / 2, j * sy + pad), (i * sx + sx - pad, j * sy + sy / 2), (i * sx + sx / 2, j * sy + sy - pad), (i * sx + pad, j * sy + sy / 2)]

            pygame.draw.polygon(screen, colors[BLOCKED], midpoints[:oppsite_dir] + midpoints[oppsite_dir + 1:])

# Draw all clusters given a board-state
def draw_clusters(screen, size, board):
    sx, sy = size[0] / board.width, size[1] / board.height
    clusters = generate_clusters(board)

    draw_board(screen, size, board)

    for cluster in clusters:
        middle = cluster.position()
        middle = (middle[0] * sx + sx / 2, middle[1] * sy + sy / 2)
        pygame.draw.circle(screen, (51, 77, 102), middle, sx / 2)
