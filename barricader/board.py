from random import randint, random
import noise
from typing import Iterator
from consts import *

# Set the x and y offset to sample simplex noise
def reseed():
    global seedx, seedy
    seedx, seedy = randint(-99999, 99999), randint(-99999, 99999)

# Random noise for generating the board, with a threshold `z`
def random_noise(x: int, y: int, z: float = 0.5):
    return int(random() > z)

# Simplex noise for generating the board, with a threshold `z`, a detail value and the number of octaves
def simplex_noise(x: int, y: int, z: float = 0.5, detail: float = 2.0, octaves: int = 4):
    freq = detail * octaves
    return int(noise.snoise2(x / freq + seedx, y / freq + seedy, octaves) * 0.5 + 0.5 > z)

# A class representing a board
class Board:
    # Create a new board with a width and height, and use the `generator_func` to generate the blocked/empty cells
    def __init__(self, w: int, h: int, generator_func: GeneratorFunc) -> None:
        self.width, self.height = w, h
        self.cells = [[generator_func(x, y) for x in range(h)] for y in range(w)]
        self.flag_clear = 5
        self.set_flag()

    # Places the flag on the map, and clears the cells around the flag
    def set_flag(self):
        flagx, flagy = randint(0, self.width - 1), self.height - 3
        self.flagpos = (flagx, flagy)

        for (x, y) in self.in_rect(self.flagpos, self.flag_clear):
            self.cells[x][y] = EMPTY
        self.cells[flagx][flagy] = FLAG

    # Place a barricade at a `coord`
    def set_barricade(self, coord: Coord):
        self.cells[coord[0]][coord[1]] = BARRICADE

    # Yield all coordinates in a rect around the `center`-coordinate with x- and y-size given by `size`
    # Yield only cell-types equal to `matching`, or all types if None
    def in_rect(self, center: Coord, size: int, matching: int | list[int] = None) -> Iterator[Coord]:
        if isinstance(matching, int):
            matching = [matching]

        for i in range(size):
            for j in range(size):
                dx, dy = i - int(size / 2), j - int(size / 2)
                x, y = center[0] + dx, center[1] + dy
                if x < 0 or x >= self.width or y < 0 or y >= self.height:
                   continue
                if matching is None or self.cells[i][j] in matching:
                    yield (x, y)

    # Yields all cells in the board if `matching` is None, or else all cells with a type equal to `matching`
    def all_cells(self, matching: int | list[int] = None) -> Iterator[Coord]:
        if isinstance(matching, int):
            matching = [matching]
        
        for i in range(self.width):
            for j in range(self.height):

                if matching is None or self.cells[i][j] in matching:
                    yield (i, j)
    
    # Yields all cells in the row of the board if `matching` is None, or else all cells with a type equal to `matching`
    def top_row(self, matching: int | list[int] = None) -> Iterator[Coord]:
        if isinstance(matching, int):
            matching = [matching]
        
        for x in range(self.width):

            if matching is None or self.cells[x][0] in matching:
                yield (x, 0)
    
    # Yields all the neighbors of a coordinate if `matching` is None, or else all the neighbor-cells with a type equal to `matching`
    def neighbors(self, coord, matching: int | list[int] = None) -> Iterator[Coord]:
        if isinstance(matching, int):
            matching = [matching]
        
        for (x, y) in [(coord[0], coord[1] - 1), (coord[0] + 1, coord[1]), (coord[0] + 0, coord[1] + 1), (coord[0] - 1, coord[1])]:

            if x < 0 or x >= self.width or y < 0 or y >= self.height:
                continue
            if matching is None or self.cells[x][y] in matching:
                yield (x, y)