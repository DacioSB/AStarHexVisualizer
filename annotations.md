# CSHARP A*

## GridFactory

### CreateRockMaze() function

1. create the gridbase
2. then from row 2 till row 7, iterate, creating a rocktype tile, in column 3
3. then from column 4 to column 8, iterate, creating rocktype tile in row 2
4. return the grid

### CreateSlimePenalty function

1. create basegrid
2. iterate over 8 positions (diagonal)
3. passing i as the col and row, and i+1 afterwards

### PlaceSlimeSafe function 
1. receives the entire grid and also the col and row which we want to place the slime
2. grid getTile from col and row
3. if tile is null, return
4. if tile is start or goal, return
5. if not, setTiletype to slime in the col/row position 