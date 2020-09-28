using UnityEngine;
using UnityEngine.UI;

// Partly taken from Brackey's FPS tutorial: https://www.youtube.com/watch?v=_QajrabyTJc
// With help from Zenya's Unity FPS Course

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    private Vector3 velocity;

    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    private bool isGrounded;

    public Text projectileCountText;
    public Text gameOverText;
    private int projectileCount = 0;
    private bool gameOver = false;

    public new Camera camera;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float finishedMazeX = 27f;

    // Start is called before the first frame update
    void Start() {
        gameOverText.text = "";
    }

    // Update is called once per frame
    void Update() {
        if (!gameOver) {
            /* x and z Movement */
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            /* y Movement */
            isGrounded = Physics.CheckSphere(
                groundCheck.position, groundDistance, groundMask
            );
            if (isGrounded && velocity.y < 0) {
                velocity.y = -2f;
            }

            if (Input.GetButtonDown("Jump") && isGrounded) {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime); // y movement

            // Throwing Projectile
            if (
                Input.GetMouseButtonDown(0)
                && projectileCount > 0
                && !GameObject.FindWithTag("Projectile") // this way only one projectile exists at a time
            ) {
                GameObject projectile = Instantiate(projectilePrefab);
                Projectile projectileScript = projectile.GetComponent<Projectile>();

                // Set properties of projectile
                projectileScript.playerCrossedAllMazes = transform.position.x > finishedMazeX;
                projectileScript.playerScript = gameObject.GetComponent<PlayerMovement>();
                // Set position and velocity of projectile
                projectile.transform.position = camera.transform.position + camera.transform.forward;
                projectile.transform.forward = camera.transform.forward;

                addProjectileCount(-1);
                GameOverMessage(
                    GameObject.FindWithTag("Pick Up") == null && projectileCount <= 0,
                    false
                ); // lose if out of projectiles
            }

            GameOverMessage(transform.position.y < -20, false); // if player is below this height you lose
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.SetActive(false);
            addProjectileCount(1);
        }
    }

    void addProjectileCount(int add) {
        projectileCount += add;
        projectileCountText.text = projectileCount.ToString();
    }

    public void GameOverMessage(bool condition, bool win) {
        if (condition) {
            gameOverText.text = "You " + (win ? "Win" : "Lose");
            Time.timeScale = 0;
            gameOver = true;
        }
    }
}
