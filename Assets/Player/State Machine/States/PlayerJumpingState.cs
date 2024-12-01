using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    private bool doubleJumped = false;

    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("<color=green>I'm now Jumping</color>");

        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        player.currentSpeed *= 0.75f;

        Vector3 movement = OrientDirectionToView(player, moveInput);
        Vector3 jumpForce = Vector3.up * player.jumpForce + movement * player.currentSpeed;
        player.rb.AddForce(jumpForce, ForceMode.Impulse);

        doubleJumped = false;
    }

    public override void UpdateState(PlayerStateManager player)
    {
        HandleMovement(player);

        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            player.currentSpeed = Mathf.Lerp(player.currentSpeed, 0, Time.deltaTime);

        RaycastHit hit;
        if (player.rb.linearVelocity.y < 0f)
        {
            if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 1.01f))
            {
                player.activeState = State.Idle;
                player.SwitchState(player.playerIdleState);
            }
            else
            {
                Debug.DrawLine(player.transform.position, player.transform.position + Vector3.down * 2f, Color.red);
            }
        }
    }

    public override void OnCollisionEnter(PlayerStateManager player)
    {
        
    }

    public override void HandleMovement(PlayerStateManager player)
    {
        if (Input.GetButtonDown("Jump") && !doubleJumped)
        {
            player.rb.linearVelocity = Vector3.zero;

            player.currentSpeed *= 0.5f;

            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 movement = OrientDirectionToView(player, moveInput);

            player.rb.AddForce(Vector3.up * player.jumpForce * 0.80f + movement * player.currentSpeed, ForceMode.Impulse);
            Debug.Log("<color=red>Double Jumping</color>");
            
            doubleJumped = true;
        }
    }

    // Orient direction input to camera direction
    public Vector3 OrientDirectionToView(PlayerStateManager player, Vector2 moveInput)
    {
        Vector3 cameraForward = player.cameraController.cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = player.cameraController.cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        return (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
    }
}
