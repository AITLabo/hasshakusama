using UnityEngine;

public class Hachisha_PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float mouseSensitivity = 2f;

    [Header("Flashlight")]
    public Light flashlight; // SpotLightを指定
    public float flashlightSmoothSpeed = 10f;

    [Header("Status")]
    public bool isRunning = false;
    public bool isCrouching = false;

    private CharacterController controller;
    private Camera cam;
    private float pitch = 0f;
    private float defaultHeight;
    public float crouchHeight = 1.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        defaultHeight = controller.height;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleActions();
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -85f, 85f);

        cam.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Status Check
        isRunning = Input.GetKey(KeyCode.LeftShift) && moveZ > 0 && !isCrouching;
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        float currentSpeed = walkSpeed;
        if (isRunning) currentSpeed = runSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;

        // Crouch height adjustment
        float targetHeight = isCrouching ? crouchHeight : defaultHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, 10f * Time.deltaTime);

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.SimpleMove(move * currentSpeed);
    }

    void HandleActions()
    {
        // Flashlight ON/OFF
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlight != null) flashlight.enabled = !flashlight.enabled;
        }

        // Flashlight Smoothing
        if (flashlight != null)
        {
            flashlight.transform.rotation = Quaternion.Slerp(flashlight.transform.rotation, cam.transform.rotation, flashlightSmoothSpeed * Time.deltaTime);
        }
    }
}
