import numpy as np
from typing import Iterator
from itertools import combinations
from consts import *

# Takes a heatmap as input and returns the coord of the most important cell
def argmax_picker(heatmap: Heatmap) -> Coord:
    flat_index = np.argmax(heatmap)
    return np.unravel_index(flat_index, heatmap.shape)

# def weighted_picker():
#     random.choices()

# Takes a board-state and a picker-function, and makes a move
def make_move(board, picker_func: PickerFunc) -> Coord:
    heatmap = generate_heatmap(board)
    # If not heatmap is uniformly equal to 0, return the move, else return None
    if np.max(heatmap) > 0:
        return picker_func(heatmap)
    return None

# Generator function which yields all coordinates in a line from `c0` to `c1`
def coords_in_line(c0, c1):
    x0, y0 = c0
    x1, y1 = c1

    dx = abs(x1 - x0)
    dy = abs(y1 - y0)
    sx = 1 if x0 < x1 else -1
    sy = 1 if y0 < y1 else -1
    err = dx - dy

    while True:
        yield (x0, y0)

        if x0 == x1 and y0 == y1:
            break

        e2 = 2 * err
        if e2 > -dy:
            err -= dy
            x0 += sx
        if e2 < dx:
            err += dx
            y0 += sy

# A cell used to calculate directions
class Cell:
    # Create a new cell from a coordinate
    # If no parent is provided a default direction and value will be set
    def __init__(self, coord, parent=None) -> None:
        self.coord = coord
        self.parent = parent
        self.dir = NODIR

        if not self.parent is None:
            dx, dy = self.parent.coord[0] - self.coord[0], self.parent.coord[1] - self.coord[1]
            self.dir = (dx != 0) * (dx + 1) + (dy != 0) * (dy + 2) - 1

        self.value = 1 if parent is None else parent.value + 1

    # Equal (==) overloader to compare two cells
    def __eq__(self, other: object) -> bool:
        return self.coord == other.coord

# Generate a direction-map, i.e. a map of ints where 0=north, 1=east, 2=south, 3=west
# The directions denote the direction from an abitrary coordinate in the direction map to `coord`
def generate_directions(board, coord: Coord) -> DirectionMap:
    directions = np.full(shape=(board.width, board.height), fill_value=NODIR)

    closed_cells = []
    open_cells = [Cell(coord)]

    while len(open_cells) > 0:

        cell = open_cells.pop(0)
        
        for coord in board.neighbors(cell.coord, [EMPTY, BARRICADE]):
            new_cell = Cell(coord, cell)
            if new_cell in closed_cells or new_cell in open_cells: continue
            open_cells.append(new_cell)

        closed_cells.append(cell)

    for cell in closed_cells:
        directions[cell.coord] = cell.dir

    return directions

# Given a direction-map, yield all coordinates from `coord` to the coordinate provided when the direction-map was generated
def retrace(directions: DirectionMap, coord: Coord) -> Iterator[Coord]:
    current = coord
    while directions[current] != NODIR:
        yield current
        x, y = current
        current = [(x, y - 1), (x + 1, y), (x, y + 1), (x - 1, y)][directions[current]]

# The sum of the difference in x and y
def manhatten_dist(c1, c2):
    dx, dy = c1[0] - c2[0], c1[1] - c2[1]
    return abs(dx) + abs(dy)

# A class to represent a cluster of blocked cells
class Cluster:
    def __init__(self, coord: Coord) -> None:
        self.coords = [coord]

    # The average position of the cluster
    def position(self) -> Coord:
        xsum, ysum = 0, 0

        for (x, y) in self.coords:
            xsum += x
            ysum += y
        
        size = len(self.coords)
        return (int(xsum / size), int(ysum / size))

    # Checks if a cluster is next to another cluster
    def next_to(self, other) -> bool:
        for myedge in self.coords:
            for otheredge in other.coords:
                if manhatten_dist(myedge, otheredge) == 1:
                    return True
        return False

    # Find the the coordinates in two clusters with the minimal distance
    def minimize_dist(self, other) -> tuple[Coord, Coord]:
        min_dist = 10000
        min_coords = None

        for myedge in self.coords:
            for otheredge in other.coords:
                dist = manhatten_dist(myedge, otheredge)
                if dist < min_dist:
                    min_dist = dist
                    min_coords = (myedge, otheredge)
        
        return min_coords

    # Merge the `other` cluster into this cluster
    def merge(self, other):
        self.coords += other.coords
        other.coords = []

    # Try merge this cluster into any other clusters
    def try_merge(self, clusters) -> bool:
        merged_into_other = False

        for cluster in clusters:
            if cluster is self: continue
            if cluster.next_to(self):
                cluster.merge(self)
                merged_into_other = True

        return merged_into_other

# Generate an array of clusters of blocked cells
# A cluster is when a valid path from any coordinate in the cluster can be reached from any other coordinate, while only moving N/E/S/W
def generate_clusters(board) -> list[Cluster]:
    clusters, final_clusters = [], []

    for coord in board.all_cells(BLOCKED):
        clusters.append(Cluster(coord))
    
    while len(clusters) > 0:
        cluster = clusters.pop(0)
        if not cluster.try_merge(clusters):
            final_clusters.append(cluster)

    return final_clusters

# Blur the heat-values in a heatmap
def apply_blur(image: Heatmap) -> Heatmap:

    kernel = np.array([[0, 1, 0],
                       [1, 2, 1],
                       [0, 1, 0]]) / 6

    padded_image = np.pad(image, ((1, 1), (1, 1)))
    result = np.zeros(image.shape)

    for i in range(image.shape[0]):
        for j in range(image.shape[1]):
            region = padded_image[i:i+3, j:j+3]
            result[i, j] = np.sum(region * kernel)

    return result

# Generate a heatmap from a board-state
def generate_heatmap(board) -> Heatmap:
    heatmap = np.ones((board.width, board.height))

    # Add importance around flag
    for coord in board.in_rect(board.flagpos, board.flag_clear):
        heatmap[coord] += 2

    # Generate direction map
    directions = generate_directions(board, board.flagpos)
    pathmask = np.zeros(heatmap.shape)

    # Add importance to cells along a valid path
    for coord in board.top_row(EMPTY):
        for pathcoord in retrace(directions, coord):
            pathmask[pathcoord] += 1

    # Normalize the pathmask
    if np.max(pathmask) > 0:
        pathmask /= np.max(pathmask)

    # Generate clusters of blocked cells
    clusters = generate_clusters(board)

    # Add importance to cells in a line between two clusters if this line crosses a path from the top of the map to the flag
    for (c0, c1) in combinations(clusters, 2):
        p0, p1 = c0.minimize_dist(c1)

        dist = manhatten_dist(p0, p1)
        if dist > 10: continue

        coords = list(coords_in_line(p0, p1))

        line_on_path = False

        for coord in coords:
            if pathmask[coord] > 0:
                line_on_path = True
                break

        if line_on_path:
            for coord in coords:
                heatmap[coord] += 50 / dist

    # Blur heatmap
    heatmap = apply_blur(heatmap)
    heatmap = apply_blur(heatmap)

    # Remove heat from flag position
    heatmap[board.flagpos] = 0

    # Remove heat from blocked and barricades
    for (x, y) in board.all_cells([BLOCKED, BARRICADE]):
        heatmap[x, y] = 0

    if np.max(heatmap) > 0:
        return heatmap / np.max(heatmap)
    return heatmap
