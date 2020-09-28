using UnityEngine;

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
}
