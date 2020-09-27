using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeLoader : MonoBehaviour {
    public int mazeRows, mazeColumns;
    public GameObject wall;
    public float size = 2f;

    public bool startDoorExists = false;
    public bool endDoorExists = false;
    public bool destroyAllInnerWalls = false;
    public Material vegetation;
    public bool highlightPath = false;
    public bool spawnProjectiles = false;
    public bool smallerFloors = false;
    public Transform pickUpContainer; // stores all the pickups, needs to be a child of a maze
    public GameObject pickUp; // when you pick it up, then it becomes a projectile

    private MazeCell[,] mazeCells;
    private Material originalFloorMaterial;

    private void Start() {
        originalFloorMaterial = wall.GetComponent<MeshRenderer>().sharedMaterial;

        InitializeMaze();

        HuntAndKillMazeAlgorithm ma = new HuntAndKillMazeAlgorithm(mazeCells);
        ma.CreateMaze();

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

        if (destroyAllInnerWalls) {
            goDestroyAllInnerWalls();
        }

        ma.dfs.findShortestPath(
            new[] { 0, mazeColumns / 2 },
            new[] { mazeRows - 1, mazeColumns / 2 }
        );
        var shortestPath = ma.dfs.getShortestPath();

        if (highlightPath) {
            for (int i = 0; i < shortestPath.Length; i++) {
                var point = shortestPath[i];

                mazeCells[point[0], point[1]].floor.GetComponent<MeshRenderer>().material = originalFloorMaterial;
            }
        }

        if (pickUpContainer != null && pickUp != null) {
            for (int i = 1; i < shortestPath.Length - 1; i++) {
                var point = shortestPath[i];
                int r = point[0];
                int c = point[1];
                Debug.Log(r + "," + c);

                if (Random.value > 0.5) {
                    GameObject pickUpObj = Instantiate(pickUp, pickUpContainer);
                    pickUpObj.transform.localPosition = new Vector3(
                        r * size,
                        -(size / 2f) + 1,
                        c * size
                    );
                    pickUpObj.name = "Pick Up (" + r + "," + c + ")";
                }
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
                if (smallerFloors) {
                    mazeCells[r, c].floor.transform.localScale *= 0.75f;
                }
                mazeCells[r, c].floor.name = "Floor (" + r + "," + c + ")";
                mazeCells[r, c].floor.transform.Rotate(Vector3.right, 90f); // turn wall into floor
                mazeCells[r, c].floor.layer = 8; // ground layer

                if (vegetation != null) {
                    mazeCells[r, c].floor.GetComponent<MeshRenderer>().material = vegetation;
                }

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

    private void goDestroyAllInnerWalls() {
        for (int r = 0; r < mazeRows; r++) {
            for (int c = 0; c < mazeColumns; c++) {
                if (r != 0) {
                    GameObject wall = mazeCells[r, c].northWall;

                    if (wall != null) {
                        GameObject.Destroy(wall);
                    }
                }

                if (r != mazeRows - 1) {
                    GameObject wall = mazeCells[r, c].southWall;

                    if (wall != null) {
                        GameObject.Destroy(wall);
                    }
                }

                if (c != 0) {
                    GameObject wall = mazeCells[r, c].westWall;

                    if (wall != null) {
                        GameObject.Destroy(wall);
                    }
                }

                if (c != mazeColumns - 1) {
                    GameObject wall = mazeCells[r, c].eastWall;

                    if (wall != null) {
                        GameObject.Destroy(wall);
                    }
                }

            }
        }
    }
}