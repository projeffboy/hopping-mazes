using UnityEngine;

// Taken from Brackey's FPS tutorial: https://www.youtube.com/watch?v=_QajrabyTJc

public class MouseLook : MonoBehaviour {
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked; // hide the cursor
    }

    void Update() {
        float mouseX = mouseValue('X');
        float mouseY = mouseValue('Y');

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private float mouseValue(char axis) {
        return Input.GetAxis("Mouse " + axis) * mouseSensitivity * Time.deltaTime;
    }
}
