using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public PlayerController playerController;
    public InputActionAsset inputActionAsset;

    private InputAction moveLeftAction;
    private InputAction moveRightAction;
    private InputAction jumpAction;

    private void OnEnable()
    {
        var playerControls = inputActionAsset.FindActionMap("PlayerControls");

        if (playerControls == null)
        {
            Debug.LogError("PlayerControls action map not found.");
            return;
        }

        moveLeftAction = playerControls.FindAction("MoveLeft");
        if (moveLeftAction != null)
        {
            moveLeftAction.performed += playerController.OnMoveLeft;
            moveLeftAction.Enable();
        }
        else
        {
            Debug.LogError("MoveLeft action not found.");
        }

        moveRightAction = playerControls.FindAction("MoveRight");
        if (moveRightAction != null)
        {
            moveRightAction.performed += playerController.OnMoveRight;
            moveRightAction.Enable();
        }
        else
        {
            Debug.LogError("MoveRight action not found.");
        }

        jumpAction = playerControls.FindAction("Jump");
        if (jumpAction != null)
        {
            jumpAction.performed += playerController.OnJump;
            jumpAction.Enable();
        }
        else
        {
            Debug.LogError("Jump action not found.");
        }
    }

    private void OnDisable()
    {
        if (moveLeftAction != null)
        {
            moveLeftAction.performed -= playerController.OnMoveLeft;
            moveLeftAction.Disable();
        }

        if (moveRightAction != null)
        {
            moveRightAction.performed -= playerController.OnMoveRight;
            moveRightAction.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.performed -= playerController.OnJump;
            jumpAction.Disable();
        }
    }
}
