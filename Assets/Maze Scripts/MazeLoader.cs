using UnityEngine;

// Followed this tutorial https://www.youtube.com/watch?v=IrO4mswO2o4 but heavily modified and improved

public class MazeLoader : MonoBehaviour {
    public int mazeRows, mazeColumns;
    public GameObject wall;
    public float size = 2f; // limit is 6f but to increase the limit, make the wall prefab larger

    public bool startDoorExists = false; // for entering a maze
    public bool endDoorExists = false; // for leaving a maze

    public bool destroyAllInnerWalls = false; // for the first maze
    public Material vegetation;
    public bool highlightPath = false; // for the first maze
    public bool spawnProjectiles = false;

    public float floorMult = 1; // smaller value to shrink the floor into a floating platform
    public bool destroyableFloors; // can the projectiles destroy the floors?

    public Transform pickUpContainer; // stores all the pickups, needs to be a child of a maze
    public GameObject pickUp; // when you pick it up, then it becomes a projectile

    private MazeCell[,] mazeCells;
    private Material originalFloorMaterial;

    [HideInInspector]
    public int[][] shortestPath;

    private void Start() {
        originalFloorMaterial = wall.GetComponent<MeshRenderer>().sharedMaterial; // we keep this to revert the highlighted path from the vegetation

        InitFloorAndWalls(); // not a maze yet

        // DFS & Hunt and Kill
        DFS dfs = new DFS(mazeRows, mazeColumns);
        HuntAndKill huntAndKill = new HuntAndKill(mazeCells); // for more info on hunt and kill, check http://jamisbuck.org/mazes/
        huntAndKill.addDfs(dfs);
        huntAndKill.GenerateMaze(); // now it's a maze
        dfs = huntAndKill.dfs;

        // Entrance and exit
        if (startDoorExists) {
            GameObject startDoor = mazeCells[0, mazeColumns / 2].northWall;
            if (startDoor != null) {
                GameObject.Destroy(startDoor);
            }
        }
        if (endDoorExists) {
            GameObject endDoor = mazeCells[mazeRows - 1, mazeColumns / 2].southWall;
            if (endDoor != null) {
                GameObject.Destroy(endDoor);
            }
        }

        // For unicursal
        if (destroyAllInnerWalls) {
            DestroyAllInnerWalls();
        }

        // DFS in action
        dfs.findShortestPath(
            new[] { 0, mazeColumns / 2 },
            new[] { mazeRows - 1, mazeColumns / 2 }
        );
        shortestPath = dfs.GetShortestPath();

        if (highlightPath) {
            for (int i = 0; i < shortestPath.Length; i++) {
                var point = shortestPath[i];

                mazeCells[point[0], point[1]].floor.GetComponent<MeshRenderer>().material = originalFloorMaterial;
            }
        }

        // Randomly generate pick ups
        GeneratePickUps(shortestPath);
    }

    private void InitFloorAndWalls() {
        mazeCells = new MazeCell[mazeRows, mazeColumns];

        /*
         * NOTE: How naming works for the maze
         * When you look at the maze, each cell has a unique (x,z) that is consistent with Unity's axes
         * When viewing the maze as a table with rows and columns, each grid instead has a (row,column)=(r,c).
         * (r,c) differs from (x,z) in that r starts from the top of the table (we'll assume r and c start from 0).
         * To make (r,c)=(x,z) the same for each grid, you have to view (r,c) sideways by tilting your head to the left.
         * As a result, number of rows correspond with the z axis and numbers of columns correspond with the x axis.
         * E.g. In a 5x5 maze, a cell at (1,3) would normally be at row 1 and column 1 (relative to row 0, column 0).
         *      But by tilting your head to the left, the cell would be at row 1 and column 3!
         * And to make it clear, north, south, east, and west correspond to the table's orientation (this applies only to the walls).
         * An alternative though, is to rotate the parent maze by 90 deg, but I didn't feel like doing that, and (0,0) would start at the top-left.
         */

        for (int r = 0; r < mazeRows; r++) {
            for (int c = 0; c < mazeColumns; c++) {
                /* CREATE ALL THE FLOORS */
                mazeCells[r, c] = new MazeCell();
                var cell = mazeCells[r, c];

                cell.floor = Instantiate(wall, transform);
                cell.floor.transform.localPosition = new Vector3(
                    r * size,
                    -size / 2f,
                    c * size
                );
                cell.floor.transform.localScale *= floorMult;
                cell.floor.name = "Floor (" + r + "," + c + ")";
                cell.floor.transform.Rotate(Vector3.left, 90f); // turn wall into floor
                cell.floor.layer = 8; // corresponds to ground layer, which the player needs to detect to jump

                if (destroyableFloors) {
                    cell.floor.tag = "Destroyable"; // destroyable by projectiles
                }
                if (vegetation != null) {
                    cell.floor.GetComponent<MeshRenderer>().material = vegetation;
                }

                /* NOTE
                 * If we created 4 walls for every tile, then there'd be double the number of inner walls.
                 * So for the north and west walls, those will only be generated by the cells at the first row and first column, respectively.
                 */

                if (r == 0) {
                    createWall(cell, "North", r, c, r * size - size / 2f, c * size, true);
                }
                createWall(cell, "South", r, c, r * size + size / 2f, c * size, true);

                if (c == 0) {
                    createWall(cell, "West", r, c, r * size, c * size - size / 2f, false);
                }
                createWall(cell, "East", r, c, r * size, c * size + size / 2f, false);
            }
        }
    }

    private void createWall(
        MazeCell cell,
        string compassDirection,
        int r,
        int c,
        float posX,
        float posZ,
        bool rotate
    ) {
        var cellWall = Instantiate(wall, transform);
        cellWall.transform.localPosition = new Vector3(posX, 0, posZ);
        cellWall.name = compassDirection + " Wall (" + r + "," + c + ")";
        if (rotate) {
            cellWall.transform.Rotate(Vector3.up * 90f);
        }

        cell.SetWall(compassDirection, cellWall);
    }

    private void DestroyAllInnerWalls() {
        for (int r = 0; r < mazeRows; r++) {
            for (int c = 0; c < mazeColumns; c++) {
                MazeCell cell = mazeCells[r, c];

                DestroyWall(cell, "north", r != 0);
                DestroyWall(cell, "south", r != mazeRows - 1);
                DestroyWall(cell, "west", c != 0);
                DestroyWall(cell, "east", c != mazeColumns - 1);
            }
        }
    }

    private void DestroyWall(MazeCell cell, string compassDirection, bool condition) {
        if (condition) {
            GameObject wall = cell.GetWall(compassDirection);

            if (wall != null) {
                GameObject.Destroy(wall);
            }
        }
    }

    private void GeneratePickUps(int[][] shortestPath) {
        if (pickUpContainer != null && pickUp != null) {
            int minPickUps = 8;

            for (int i = 1; i < shortestPath.Length - 1; i++) {
                var point = shortestPath[i];
                int r = point[0];
                int c = point[1];

                if (
                    Random.value < 0.5
                    || minPickUps >= shortestPath.Length - 1 - i // in case random fails to generate at least 8 pick ups, which rarely happens
                ) {
                    GameObject pickUpObj = Instantiate(pickUp, pickUpContainer);
                    pickUpObj.transform.localPosition = size * new Vector3(r, 0, c);
                    pickUpObj.name = "Pick Up (" + r + "," + c + ")";
                    minPickUps--;
                }
            }
        }
    }
}