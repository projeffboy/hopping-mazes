using UnityEngine;

// Followed this tutorial https://www.youtube.com/watch?v=IrO4mswO2o4

public class MazeCell {
    public GameObject
        northWall,
        southWall,
        eastWall,
        westWall,
        floor;
    public bool visited = false;

    public GameObject GetWall(string compassDirection) {
        switch (compassDirection.ToLower()) {
            case "north":
                return northWall;
            case "south":
                return southWall;
            case "east":
                return eastWall;
            case "west":
                return westWall;
        }

        return null;
    }

    public void SetWall(string compassDirection, GameObject obj) {
        switch (compassDirection.ToLower()) {
            case "north":
                northWall = obj;
                break;
            case "south":
                southWall = obj;
                break;
            case "east":
                eastWall = obj;
                break;
            case "west":
                westWall = obj;
                break;
        }
    }
}
