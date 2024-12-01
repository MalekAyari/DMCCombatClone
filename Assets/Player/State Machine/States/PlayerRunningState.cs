using Unity.Mathematics;
using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("I'm now Running");
    }

    public override void UpdateState(PlayerStateManager player)
    {
        HandleMovement(player);
    }

    public override void OnCollisionEnter(PlayerStateManager player)
    {
        //TODO: Collision handling
    }

    public override void HandleMovement(PlayerStateManager player)
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool isMoving = moveInput.magnitude > 0;
        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isMoving 
            ? (sprinting ? player.speed * player.sprintMultiplier : player.speed) 
            : 0;

        player.currentSpeed = Mathf.Lerp(player.currentSpeed, targetSpeed, Time.deltaTime * player.sprintSmoothSpeed);

        Vector3 movement = OrientDirectionToView(player, moveInput);
        
        if (player.lockedOn){
            OrientCharacterToTarget(player, player.LockedTarget.position);
        } else {
            OrientCharacterToDirection(player, movement);
        }

        player.rb.MovePosition(player.rb.position + movement * player.currentSpeed * Time.deltaTime);

        if (player.currentSpeed <= 1f) 
        {
            player.activeState = State.Idle;
            player.SwitchState(player.playerIdleState);
        }

        if (Input.GetButtonDown("Jump"))
        {
            player.activeState = State.Jumping;
            player.SwitchState(player.playerJumpingState);
        }
    }

    //Used by OrientCharacterToDirection to get normalized direction
    private Vector3 OrientDirectionToView(PlayerStateManager player, Vector2 moveInput)
    {
        Vector3 cameraForward = player.cameraController.cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = player.cameraController.cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        return (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
    }

    // If there's movement, rotate the character to face the direction of movement
    private void OrientCharacterToDirection(PlayerStateManager player, Vector3 movementDirection)
    {
        if (movementDirection.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            player.character.transform.rotation = Quaternion.Slerp(player.character.transform.rotation, targetRotation, Time.deltaTime * player.rotationSpeed);
        }
    }

    private void OrientCharacterToTarget(PlayerStateManager player, Vector3 target)
    {
        Vector3 directionToTarget = (target - player.character.transform.position).normalized;
        directionToTarget.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * player.rotationSpeed);
    }

    // Orient direction input to camera direction
    
}
