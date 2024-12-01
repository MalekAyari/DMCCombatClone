using System;
using UnityEngine;

[Serializable]
public enum State {
    Running,
    Idle,
    Jumping,
}

public class PlayerStateManager : MonoBehaviour
{
    [Header("References")]
    public ThirdPersonCameraController cameraController;
    public Transform character;
    public Rigidbody rb;
    
    [Header("Configs")]
    public float speed = 3f;
    public float rotationSpeed = 3f;
    public float sprintMultiplier = 2.5f;
    public float jumpForce = 10f;
    public float sprintSmoothSpeed = 0.2f;
    public float currentSpeed = 0f;

    [Header("Info")]
    public State activeState = State.Idle;
    public bool lockedOn;
    public Transform LockedTarget;

    //States
    PlayerBaseState currentState;
    public PlayerRunningState playerRunningState = new PlayerRunningState();
    public PlayerIdleState playerIdleState = new PlayerIdleState();
    public PlayerJumpingState playerJumpingState = new PlayerJumpingState();

    void Start()
    {
        currentState = playerIdleState;

        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(PlayerBaseState state){
        currentState = state;
        
        state.EnterState(this);
    }

    
}
