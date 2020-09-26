using UnityEngine;

public class MazeCell {
    public bool visited = false;
    public GameObject
        northWall,
        southWall,
        eastWall,
        westWall,
        floor;

    public GameObject getWall(string compassPoint) {
        switch (compassPoint) {
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
