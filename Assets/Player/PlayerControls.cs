using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float speed = 2.5f;
    public float sprintMultiplier = 3f;
    public float jumpForce = 500f;
    public float mouseSensitivity = 100f;
    public float cameraDistance = 5f;
    public float minCameraDistance = 2f; // Minimum camera distance
    public float cameraSmoothSpeed = 7.5f;
    public float sprintSmoothSpeed = 0.2f;

    public Transform cameraTransform;

    public Vector2 cameraDegreeConstraints = new Vector2(-60, 60);

    private Rigidbody rb;
    private bool isGrounded;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = speed;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        bool sprinting = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = sprinting ? speed * sprintMultiplier : speed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * sprintSmoothSpeed);

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 movement = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    void Update()
    {
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation = (yRotation + mouseX) % 360f;
        if (yRotation < 0) yRotation += 360f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, cameraDegreeConstraints.x, cameraDegreeConstraints.y);

        Quaternion desiredRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        Vector3 desiredCameraPosition = transform.position + desiredRotation * new Vector3(0, 0, -cameraDistance);

        Vector3 directionToCamera = (desiredCameraPosition - transform.position).normalized;
        float currentDistance = Vector3.Distance(transform.position, desiredCameraPosition);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, directionToCamera, out hit, currentDistance))
        {
            desiredCameraPosition = hit.point - directionToCamera * 0.1f; 
        }

        float adjustedDistance = Vector3.Distance(transform.position, desiredCameraPosition);
        if (adjustedDistance < minCameraDistance)
        {
            desiredCameraPosition = transform.position + directionToCamera * minCameraDistance;
        }

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredCameraPosition, Time.deltaTime * cameraSmoothSpeed);
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, desiredRotation, Time.deltaTime * cameraSmoothSpeed);

        cameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
