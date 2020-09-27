using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    bool isGrounded;

    public Text projectileCountText;
    public Text gameOverText;
    private int projectileCount = 0;
    private bool gameOver = false;

    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public Camera camera;

    // Start is called before the first frame update
    void Start() {
        gameOverText.text = "";
    }

    // Update is called once per frame
    void Update() {
        if (!gameOver) {
            isGrounded = Physics.CheckSphere(
                groundCheck.position, groundDistance, groundMask
            );

            if (isGrounded && velocity.y < 0) {
                velocity.y = -2f;
            }
            
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded) {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            if (Input.GetMouseButtonDown(0) && projectileCount > 0) {
                GameObject projectile = Instantiate(projectilePrefab);
                projectile.transform.position = camera.transform.position + camera.transform.forward;
                projectile.transform.forward = camera.transform.forward;

                projectileCount--;
                projectileCountText.text = projectileCount.ToString();
                gameOverMessage(GameObject.FindWithTag("Pick Up") == null && projectileCount <= 0);
            }

            gameOverMessage(transform.position.y < -20);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Pick Up")) {
            other.gameObject.SetActive(false);
            projectileCount++;
            projectileCountText.text = projectileCount.ToString();
        }
    }

    void gameOverMessage(bool condition) {
        if (condition) {
            gameOverText.text = "You Lose";
            Time.timeScale = 0;
            gameOver = true;
        }
    }
}
