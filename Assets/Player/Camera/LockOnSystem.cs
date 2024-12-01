using UnityEngine;
using System.Collections.Generic;

public class LockOnSystem : MonoBehaviour
{

    [Header("References")]
    public Transform cameraTransform;
    public Transform actionCamera;
    public Transform playerTransform;
    public PlayerStateManager playerState;

    [Header("Settings")]
    public float lockOnRange = 15f; 
    public LayerMask targetLayer; 
    public float actionCameraSmoothSpeed = 5f;
    public float horizontalOffset = 4.5f;
    [Range(0.2f, 0.8f)]
    public float sideSwitchThreshold = 0.5f;

    private Transform lockedTarget;
    private bool isLockedOn = false;
    private List<Transform> potentialTargets = new List<Transform>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isLockedOn)
                UnlockTarget();
            else
                LockOnTarget();
        }

        if (isLockedOn && Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0) 
        {
            CycleTargets(); 
        }

        if (isLockedOn && lockedTarget != null)
        {
            //TODO: Orient camera for action shot
        }
    }

    private void LockOnTarget()
    {
        Collider[] hits = Physics.OverlapSphere(playerTransform.position, lockOnRange, targetLayer);
        potentialTargets.Clear();

        foreach (var hit in hits)
        {
            potentialTargets.Add(hit.transform);
        }

        if (potentialTargets.Count > 0)
        {
            lockedTarget = GetClosestTarget();
            isLockedOn = true;
            playerState.lockedOn = true;
            playerState.LockedTarget = lockedTarget;
            Debug.Log($"Locked onto: {lockedTarget.name}");
        }
    }

    private void UnlockTarget()
    {
        lockedTarget = null;
        isLockedOn = false;
        playerState.lockedOn = false;
        playerState.LockedTarget = null;
        Debug.Log("Lock-On Released");
    }

    private void CycleTargets()
    {
        if (potentialTargets.Count <= 1) return;

        float scrollDirection = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scrollDirection, 0f)) return;

        Transform newTarget = null;
        float smallestAngle = Mathf.Infinity;
        Vector3 referenceDirection = lockedTarget != null 
            ? (lockedTarget.position - cameraTransform.position).normalized 
            : cameraTransform.forward;

        foreach (Transform target in potentialTargets)
        {
            if (target == lockedTarget) continue;

            Vector3 toTarget = (target.position - cameraTransform.position).normalized;
            float lateralDot = Vector3.Dot(cameraTransform.right, toTarget);

            if ((scrollDirection > 0 && lateralDot > 0) || (scrollDirection < 0 && lateralDot < 0))
            {
                float angle = Vector3.Angle(referenceDirection, toTarget);
                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    newTarget = target;
                }
            }
        }

        if (newTarget != null)
        {
            playerState.LockedTarget = newTarget;
            lockedTarget = newTarget;
            Debug.Log($"Switched Lock-On to: {lockedTarget.name}");
        }
    }


    private Transform GetClosestTarget()
    {
        Transform closest = null;
        float highestDot = -Mathf.Infinity;

        foreach (Transform target in potentialTargets)
        {
            Vector3 directionToTarget = (target.position - cameraTransform.position).normalized;
            float dot = Vector3.Dot(cameraTransform.forward, directionToTarget);

            if (dot > highestDot)
            {
                highestDot = dot;
                closest = target;
            }
        }

        return closest;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isLockedOn ? Color.green : Color.red;
        Gizmos.DrawWireSphere(playerTransform.position, lockOnRange);
    }
}
