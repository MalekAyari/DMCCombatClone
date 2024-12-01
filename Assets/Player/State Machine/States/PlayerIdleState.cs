using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player){
        Debug.Log("I'm now Idle");
    }

    public override void UpdateState(PlayerStateManager player){
        HandleMovement(player);

        if (player.lockedOn)
            OrientCharacterToTarget(player, player.LockedTarget);
    }

    public override void OnCollisionEnter(PlayerStateManager player){

    }

    public override void HandleMovement(PlayerStateManager player){
        
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            player.activeState = State.Running;
            player.SwitchState(player.playerRunningState);
        }            
        
        if (Input.GetButtonDown("Jump")){
            player.activeState = State.Jumping;
            player.SwitchState(player.playerJumpingState);
        }
    }

    private void OrientCharacterToTarget(PlayerStateManager player, Transform target)
    {
        Vector3 directionToTarget = (target.position - player.transform.position).normalized;
        directionToTarget.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        player.transform.rotation = Quaternion.Slerp(
            player.transform.rotation, 
            targetRotation, 
            Time.deltaTime * player.rotationSpeed
        );
    }
}
