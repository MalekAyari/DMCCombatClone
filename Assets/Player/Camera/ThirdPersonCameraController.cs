using Unity.Mathematics;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public LockOnSystem lockOnSystem;
    public PlayerStateManager player;

    [Header("Control Settings")]
    public float mouseSensitivity = 500f;
    public Vector2 cameraAngleConstraints = new Vector2(-60, 60);

    [Header("Camera Properties")]

    [Range(4f, 8f)]
    public float maxCameraDistance = 5f;
    public float minCameraDistance = 2f;
    public float cameraSmoothSpeed = 7.5f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (player.lockedOn)
            HandleActionCam();
        else
            HandleFreeCam();
    }

    void HandleFreeCam()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation = (yRotation + mouseX) % 360f;
        if (yRotation < 0) yRotation += 360f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, cameraAngleConstraints.x, cameraAngleConstraints.y);

        Quaternion desiredRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        Vector3 desiredCameraPosition = transform.position + desiredRotation * new Vector3(0, 0, -maxCameraDistance);

        // Obstruction raycast
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

    void HandleActionCam()
    {
        if (Input.GetAxis("Mouse X") > lockOnSystem.sideSwitchThreshold && lockOnSystem.actionCamera.localPosition.x < 0)
            lockOnSystem.actionCamera.localPosition = new Vector3(lockOnSystem.horizontalOffset, lockOnSystem.actionCamera.localPosition.y, lockOnSystem.actionCamera.localPosition.z);
        else if (Input.GetAxis("Mouse X") < -lockOnSystem.sideSwitchThreshold && lockOnSystem.actionCamera.localPosition.x > 0)
            lockOnSystem.actionCamera.localPosition = new Vector3(-lockOnSystem.horizontalOffset, lockOnSystem.actionCamera.localPosition.y, lockOnSystem.actionCamera.localPosition.z);


        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            lockOnSystem.actionCamera.position,
            Time.deltaTime * lockOnSystem.actionCameraSmoothSpeed
        );

        Vector3 targetDirection = player.LockedTarget.position + Vector3.up * 1.5f - cameraTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        player.character.LookAt(player.LockedTarget);
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, Time.deltaTime * lockOnSystem.actionCameraSmoothSpeed);
    }

}
