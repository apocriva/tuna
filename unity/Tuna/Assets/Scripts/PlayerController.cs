using UnityEngine;
using UnityEngine.InputSystem;

public static class VectorExtensions
{

    public static Vector3 AsXZ(this Vector2 v) => new(v.x, 0f, v.y);

}

[RequireComponent(typeof(BlockMovement))]
public class PlayerController : MonoBehaviour
{

    private const float MoveInputMinimum = 0.5f;
    private const float LookInputMinimum = 0.5f;

    private Vector2 currentMoveVector = Vector3.zero;
    private Vector2 currentLookVector = Vector2.zero;
    private BlockMovement blockMovement;

    public void Start()
    {
        blockMovement = GetComponent<BlockMovement>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var moveInput = context.ReadValue<Vector2>();
        if (moveInput.magnitude < MoveInputMinimum)
        {
            currentMoveVector = Vector2.zero;
            return;
        }

        currentMoveVector = new Vector2(moveInput.x, moveInput.y);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        var lookInput = context.ReadValue<Vector2>();
        if (lookInput.magnitude < LookInputMinimum)
        {
            currentLookVector = Vector2.zero;
            return;
        }

        currentLookVector = lookInput;
    }

    public void Update()
    {
        UpdateMoveInput();
        UpdateLookInput();
    }

    private void UpdateMoveInput()
    {
        if (currentMoveVector == Vector2.zero)
            return;

        var angle = Vector3.SignedAngle(Vector3.forward, currentMoveVector.AsXZ(), Vector3.up);
        var direction = DirectionUtility.FromAngle(angle);
        blockMovement.MoveInDirection(direction);
    }

    private void UpdateLookInput()
    {
        if (currentLookVector == Vector2.zero)
            return;

        if (currentLookVector.x > 0f)
            blockMovement.TurnInDirection(Direction.Left);
        else if (currentLookVector.x < 0f)
            blockMovement.TurnInDirection(Direction.Right);
    }

}
