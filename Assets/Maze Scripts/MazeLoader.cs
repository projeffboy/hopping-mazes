using UnityEngine;
using System.Collections;

public class MazeLoader : MonoBehaviour {
    public int mazeRows, mazeColumns;
    public GameObject wall;
    public float size = 2f;

    public bool startDoorExists = false;
    public int startDoorRow;
    public int startDoorColumn;
    public bool endDoorExists = false;
    public int endDoorRow;
    public int endDoorColumn;


    private MazeCell[,] mazeCells;

    private void Start() {
        InitializeMaze();

        MazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells);
        ma.CreateMaze();

        if (startDoorExists) {
            GameObject startDoor = mazeCells[startDoorRow, startDoorColumn].northWall;
            if (startDoor != null) {
                GameObject.Destroy(startDoor);
            }
        }
        if (endDoorExists) {
            GameObject endDoor = mazeCells[endDoorRow, endDoorColumn].southWall;
            if (endDoor != null) {
                GameObject.Destroy(endDoor);
            }
        }
    }

    private void InitializeMaze() {
        mazeCells = new MazeCell[mazeRows, mazeColumns];

        for (int r = 0; r < mazeRows; r++) {
            for (int c = 0; c < mazeColumns; c++) {
                mazeCells[r, c] = new MazeCell();
                mazeCells[r, c].floor = Instantiate(wall, transform) as GameObject;
                mazeCells[r, c].floor.transform.localPosition = new Vector3(
                    r * size,
                    -(size / 2f),
                    c * size
                );
                mazeCells[r, c].floor.name = "Floor (" + r + "," + c + ")";
                mazeCells[r, c].floor.transform.Rotate(Vector3.right, 90f); // turn wall into floor
                mazeCells[r, c].floor.layer = 8; // ground layer

                if (c == 0) {
                    mazeCells[r, c].westWall = Instantiate(wall, transform) as GameObject;
                    mazeCells[r, c].westWall.transform.localPosition = new Vector3(
                        r * size,
                        0,
                        c * size - size / 2f
                    );
                    mazeCells[r, c].westWall.name = "West Wall (" + r + "," + c + ")";
                }

                mazeCells[r, c].eastWall = Instantiate(wall, transform) as GameObject;
                mazeCells[r, c].eastWall.transform.localPosition = new Vector3(
                    r * size,
                    0,
                    c * size + size / 2f
                );
                mazeCells[r, c].eastWall.name = "East Wall (" + r + "," + c + ")";

                if (r == 0) {
                    mazeCells[r, c].northWall = Instantiate(wall, transform) as GameObject;
                    mazeCells[r, c].northWall.transform.localPosition = new Vector3(
                        r * size - size / 2f,
                        0,
                        c * size
                    );
                    mazeCells[r, c].northWall.name = "North Wall (" + r + "," + c + ")";
                    mazeCells[r, c].northWall.transform.Rotate(Vector3.up * 90f);
                }

                mazeCells[r, c].southWall = Instantiate(wall, transform) as GameObject;
                mazeCells[r, c].southWall.transform.localPosition = new Vector3(
                    r * size + size / 2f,
                    0,
                    c * size
                );
                mazeCells[r, c].southWall.name = "South Wall (" + r + "," + c + ")";
                mazeCells[r, c].southWall.transform.Rotate(Vector3.up * 90f);
            }
        }
    }
}