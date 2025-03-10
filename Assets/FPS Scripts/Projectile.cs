using UnityEngine;
using UnityEngine.UI;

// With help from Zenya's Unity FPS Course

public class Projectile : MonoBehaviour {
    public float speed = 40f;
    public int lifeDuration = 2;

    [HideInInspector]
    public bool playerCrossedAllMazes; // set by player script
    [HideInInspector]
    public PlayerMovement playerScript; // to call game over message

    private float lifeTimer;
    private int mazeSize = 6; // if i wrote this better I'd have it reference to the size in MazeLoader

    void Start() {
        lifeTimer = lifeDuration;
    }

    void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f) {
            AmIOutOfAmmo();

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        // On contact with floors
        if (other.gameObject.CompareTag("Destroyable")) {
            GameObject maze = other.transform.parent.gameObject;
            MazeLoader script = maze.GetComponent<MazeLoader>();

            int row = (int) other.transform.localPosition.x / mazeSize;
            int column = (int) other.transform.localPosition.z / mazeSize;

            for (int i = 0; i < script.shortestPath.Length; i++) {
                var point = script.shortestPath[i];
                // Debug.Log(row + "," + column + "==" + point[0] + "," + point[1]);

                // if you shoot a floor that is part of the shortest path, you either win or lose depending on which side of the destroyable maze you are on
                playerScript.GameOverMessage(row == point[0] && column == point[1], playerCrossedAllMazes);
            }

            Destroy(other.gameObject);
        }

        // Prevent player from colliding with projectile
        if (!other.gameObject.CompareTag("Player") /* && !other.gameObject.CompareTag("Pick Up") */) {
            AmIOutOfAmmo();
            Destroy(gameObject);
        }
    }

    private bool AmIOutOfAmmo() { // lose if you are and the pick up is gone
        bool condition = GameObject.FindWithTag("Pick Up") == null
            && playerScript.projectileCount <= 0;

        playerScript.GameOverMessage(condition, false);

        return condition;
    }
}
