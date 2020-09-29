using System.Collections;
using UnityEngine;

// Followed this tutorial https://www.youtube.com/watch?v=IrO4mswO2o4
// As per TA's request, I've written the pseudocode on a piece of paper then retyped my implementation of it here

enum Compass {
    North,
    South,
    East,
    West
};  

public class HuntAndKill {
    private MazeCell[,] mazeCells;
    private int mazeRows, mazeColumns;

    // so the algorithm starts at (0,0)
    private int currentRow = 0;
    private int currentColumn = 0;
    private bool finishedKilling = false;

    public DFS dfs; // optional, does not need to be set

    public HuntAndKill(MazeCell[,] mazeCells) {
        this.mazeCells = mazeCells;

        mazeRows = mazeCells.GetLength(0);
        mazeColumns = mazeCells.GetLength(1);
    }

    public void addDfs(DFS dfs) {
        this.dfs = dfs;
    }

    public void GenerateMaze() {
        mazeCells[currentRow, currentColumn].visited = true;

        while(!finishedKilling) {
            KillingSpree(); // terminates when it hits dead end
            Hunt(); // find next unvisited cell for the next killing spree
        }
    }

    private void KillingSpree() {
        while (RouteStillAvailable(currentRow, currentColumn)) {
            Compass direction = (Compass) Random.Range(0, 4);

            if (direction == Compass.North && ValidAndUnvisitedCell(currentRow - 1, currentColumn)) {
                // north as in destroy the south wall of the cell above
                DestroyWall(currentRow - 1, currentColumn, "south");
                currentRow--;
            } else if (direction == Compass.South && ValidAndUnvisitedCell(currentRow + 1, currentColumn)) {
                DestroyWall(currentRow, currentColumn, "south");
                currentRow++;
            } else if (direction == Compass.West && ValidAndUnvisitedCell(currentRow, currentColumn - 1)) {
                // west as in destroy the east wall of the cell to its left
                DestroyWall(currentRow, currentColumn - 1, "east");
                currentColumn--;
            } else if (direction == Compass.East && ValidAndUnvisitedCell(currentRow, currentColumn + 1)) {
                DestroyWall(currentRow, currentColumn, "east");
                currentColumn++;
            }

            mazeCells[currentRow, currentColumn].visited = true;
        }
    }

    private void Hunt() {
        finishedKilling = true; // just an assumption

        // hunt for surviving cells
        for (int r = 0; r < mazeRows; r++) {
            for (int c = 0; c < mazeColumns; c++) {
                if (!mazeCells[r, c].visited && HasAdjVisitedCell(r, c)) { // found one >:)
                    finishedKilling = false;
                    DestroyAdjWall(r, c);
                    mazeCells[r, c].visited = true;
                    return;
                }
            }
        }
    }

    private bool RouteStillAvailable(int row, int column) {
        // look four ways for an unvisited cell
        return (row > 0 && !mazeCells[row - 1, column].visited)
            || (row < mazeRows - 1 && !mazeCells[row + 1, column].visited)
            || (column > 0 && !mazeCells[row, column - 1].visited)
            || (column < mazeColumns - 1 && !mazeCells[row, column + 1].visited);
    }

    private bool ValidAndUnvisitedCell(int row, int column) {
            // make sure it doesn't go out of bounds
        return 0 <= row && row < mazeRows
            && 0 <= column && column < mazeColumns
            // unvisited
            && !mazeCells[row, column].visited;
    }

    private void DestroyWall(int row, int column, string compassDirection) {
        GameObject wall = mazeCells[row, column].GetWall(compassDirection);

        if (wall != null) {
            wall.SetActive(false);

            if (dfs != null) {
                int linkedRow = row;
                int linkedColumn = column;

                switch (compassDirection) {
                    case "south":
                        linkedRow += 1;
                        break;
                    case "east":
                        linkedColumn += 1;
                        break;
                }
                dfs.addEdge(new[] { row, column }, new[] { linkedRow, linkedColumn }); // delete wall = add edge
            }
        }
    }

    private bool HasAdjVisitedCell(int row, int column) {
        // check all 4 adj cells
        return (row > 0 && mazeCells[row - 1, column].visited)
            || (row < (mazeRows - 2) && mazeCells[row + 1, column].visited)
            || (column > 0 && mazeCells[row, column - 1].visited)
            || (column < (mazeColumns - 2) && mazeCells[row, column + 1].visited);
    }

    private void DestroyAdjWall(int row, int column) {
        bool destroyedWall = false;

        while (!destroyedWall) {
            Compass direction = (Compass)Random.Range(0, 4);

            if (
                direction == Compass.North
                && row > 0
                && mazeCells[row - 1, column].visited
            ) {
                // north as in destroy the south wall of the cell above
                DestroyWall(row - 1, column, "south");
            } else if (
                direction == Compass.South
                && row < mazeRows - 2
                && mazeCells[row + 1, column].visited
            ) {
                DestroyWall(row, column, "south");
            } else if (
                direction == Compass.West
                && column > 0
                && mazeCells[row, column - 1].visited
            ) {
                // west as in destroy the east wall of the cell to its left
                DestroyWall(row, column - 1, "east");
            } else if (
                direction == Compass.East
                && column < mazeColumns - 2
                && mazeCells[row, column + 1].visited
            ) {
                DestroyWall(row, column, "east");
            } else {
                continue;
            }

            destroyedWall = true;
        }
    }
}
