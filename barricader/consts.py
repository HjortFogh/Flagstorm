import numpy as np
from typing import Callable

Coord = tuple[int, int]
Heatmap = np.ndarray
DirectionMap = np.ndarray

GeneratorFunc = Callable[[int, int], int]
PickerFunc = Callable[[Heatmap], Coord]

EMPTY = 0
BLOCKED = 1
BARRICADE = 2
FLAG = 3
MAX_CELL = 4

NORTH = 0
EAST = 1
SOUTH = 2
WEST = 3
NODIR = 4
