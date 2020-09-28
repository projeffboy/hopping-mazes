using System.Collections;
using UnityEngine;

public class HuntAndKill {
    private MazeCell[,] mazeCells;
    private int mazeRows, mazeColumns;

    private int currentRow = 0;
    private int currentColumn = 0;

    private bool courseComplete = false;

    public DFS dfs;

    public HuntAndKill(MazeCell[,] mazeCells) {
        this.mazeCells = mazeCells;

        mazeRows = mazeCells.GetLength(0);
        mazeColumns = mazeCells.GetLength(1);
    }

    public void CreateMaze() {
        dfs = new DFS(mazeRows, mazeColumns);
        KillThenHunt();
    }

    private void KillThenHunt() {
        mazeCells[currentRow, currentColumn].visited = true;

        while(!courseComplete) {
            Kill(); // run until hit dead end
            Hunt(); // find next unvisited cell with adj visited cell. Otherwise, end
        }
    }

    private void Kill() {
        while (RouteStillAvailable(currentRow, currentColumn)) {
            int direction = Random.Range (1, 5);

            if (direction == 1 && CellIsAvailable(currentRow - 1, currentColumn)) {
                // North
                DestroyWallIfItExists(currentRow, currentColumn, "north");
                DestroyWallIfItExists(currentRow - 1, currentColumn, "south");
                currentRow--;
            } else if (direction == 2 && CellIsAvailable(currentRow + 1, currentColumn)) {
                // South
                DestroyWallIfItExists(currentRow, currentColumn, "south");
                DestroyWallIfItExists(currentRow + 1, currentColumn, "north");
                currentRow++;
            } else if (direction == 3 && CellIsAvailable(currentRow, currentColumn + 1)) {
                // east
                DestroyWallIfItExists(currentRow, currentColumn, "east");
                DestroyWallIfItExists(currentRow, currentColumn + 1, "west");
                currentColumn++;
            } else if (direction == 4 && CellIsAvailable(currentRow, currentColumn - 1)) {
                // west
                DestroyWallIfItExists(currentRow, currentColumn, "west");
                DestroyWallIfItExists(currentRow, currentColumn - 1, "east");
                currentColumn--;
            }

            mazeCells[currentRow, currentColumn].visited = true;
        }
    }

    private void Hunt() {
        courseComplete = true; // Set it to this, and see if we can prove otherwise below!

        for (int r = 0; r < mazeRows; r++) {
            for (int c = 0; c < mazeColumns; c++) {
                if (!mazeCells[r, c].visited && CellHasAnAdjacentVisitedCell(r, c)) {
                    courseComplete = false; // Yep, we found something so definitely do another Kill cycle.
                    currentRow = r;
                    currentColumn = c;
                    DestroyAdjacentWall(currentRow, currentColumn);
                    mazeCells[currentRow, currentColumn].visited = true;
                    return; // Exit the function
                }
            }
        }
    }


    private bool RouteStillAvailable(int row, int column) {
        int availableRoutes = 0;

        if (row > 0 && !mazeCells[row - 1, column].visited) {
            availableRoutes++;
        }

        if (row < mazeRows - 1 && !mazeCells[row + 1, column].visited) {
            availableRoutes++;
        }

        if (column > 0 && !mazeCells[row, column - 1].visited) {
            availableRoutes++;
        }

        if (column < mazeColumns - 1 && !mazeCells[row, column + 1].visited) {
            availableRoutes++;
        }

        return availableRoutes > 0;
    }

    private bool CellIsAvailable(int row, int column) {
        if (row >= 0 && row < mazeRows && column >= 0 && column < mazeColumns && !mazeCells[row, column].visited) {
            return true;
        } else {
            return false;
        }
    }

    private void DestroyWallIfItExists(int row, int column, string compassPoint) {
        GameObject wall = mazeCells[row, column].GetWall(compassPoint);

        if (wall != null) {
            GameObject.Destroy(wall);

            int linkedRow = row;
            int linkedColumn = column;
            switch (compassPoint) {
                case "north":
                    linkedRow -= 1;
                    break;
                case "south":
                    linkedRow += 1;
                    break;
                case "east":
                    linkedColumn += 1;
                    break;
                case "west":
                    linkedColumn -= 1;
                    break;
            }
            dfs.addEdge(new[] { row, column }, new[] { linkedRow, linkedColumn });
        }
    }

    private bool CellHasAnAdjacentVisitedCell(int row, int column) {
        int visitedCells = 0;

        // Look 1 row up (north) if we're on row 1 or greater
        if (row > 0 && mazeCells[row - 1, column].visited) {
            visitedCells++;
        }

        // Look one row down (south) if we're the second-to-last row (or less)
        if (row < (mazeRows - 2) && mazeCells[row + 1, column].visited) {
            visitedCells++;
        }

        // Look one row left (west) if we're column 1 or greater
        if (column > 0 && mazeCells[row, column - 1].visited) {
            visitedCells++;
        }

        // Look one row right (east) if we're the second-to-last column (or less)
        if (column < (mazeColumns - 2) && mazeCells[row, column + 1].visited) {
            visitedCells++;
        }

        // return true if there are any adjacent visited cells to this one
        return visitedCells > 0;
    }

    private void DestroyAdjacentWall(int row, int column) {
        bool wallDestroyed = false;

        while (!wallDestroyed) {
            int direction = Random.Range (1, 5);

            if (direction == 1 && row > 0 && mazeCells[row - 1, column].visited) {
                DestroyWallIfItExists(row, column, "north");
                DestroyWallIfItExists(row - 1, column, "south");
                wallDestroyed = true;
            } else if (direction == 2 && row < (mazeRows - 2) && mazeCells[row + 1, column].visited) {
                DestroyWallIfItExists(row, column, "south");
                DestroyWallIfItExists(row + 1, column, "north");
                wallDestroyed = true;
            } else if (direction == 3 && column > 0 && mazeCells[row, column - 1].visited) {
                DestroyWallIfItExists(row, column, "west");
                DestroyWallIfItExists(row, column - 1, "east");
                wallDestroyed = true;
            } else if (direction == 4 && column < (mazeColumns - 2) && mazeCells[row, column + 1].visited) {
                DestroyWallIfItExists(row, column, "east");
                DestroyWallIfItExists(row, column + 1, "west");
                wallDestroyed = true;
            }
        }

    }
}
