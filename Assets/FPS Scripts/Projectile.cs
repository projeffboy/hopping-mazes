using UnityEngine;
using UnityEngine.UI;

// With help from Zenya's Unity FPS Course

public class Projectile : MonoBehaviour {
    public float speed = 30f;
    public int lifeDuration = 2;

    [HideInInspector]
    public bool playerCrossedAllMazes; // set by player script
    [HideInInspector]
    public Text gameOverText; // set by player script

    private float lifeTimer;
    private bool destroyedSomething = false;
    private int mazeSize = 6; // if i wrote this better I'd have it reference to the size in MazeLoader

    void Start() {
        lifeTimer = lifeDuration;
    }

    void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        // On contact with floors
        if (other.gameObject.CompareTag("Destroyable") && !destroyedSomething) {
            GameObject maze = other.transform.parent.gameObject;
            MazeLoader script = maze.GetComponent<MazeLoader>();

            int row = (int) other.transform.localPosition.x / mazeSize;
            int column = (int) other.transform.localPosition.z / mazeSize;

            for (int i = 0; i < script.shortestPath.Length; i++) {
                var point = script.shortestPath[i];
                // Debug.Log(row + "," + column + "==" + point[0] + "," + point[1]);

                if (row == point[0] && column == point[1]) {
                    // Game Over
                    if (playerCrossedAllMazes) {
                        gameOverText.text = "You Win";
                    } else {
                        gameOverText.text = "You Lose";
                    }
                    Time.timeScale = 0;
                }
            }

            Destroy(other.gameObject);
            destroyedSomething = true;
        }

        // Prevent player from colliding with projectile
        if (!other.gameObject.CompareTag("Player") /* && !other.gameObject.CompareTag("Pick Up") */) {
            Destroy(gameObject);
        }
    }
}
