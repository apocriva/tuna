using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BlockMovement))]
public class PlayerController : MonoBehaviour
{

    private const float MoveInputMinimum = 0.5f;

    private Vector3 currentMoveVector = Vector3.zero;
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
            currentMoveVector = Vector3.zero;
            return;
        }

        Debug.Log($"OnMove: {moveInput}");
        currentMoveVector = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    public void Update()
    {
        UpdateMoveInput();
    }

    private void UpdateMoveInput()
    {
        if (currentMoveVector == Vector3.zero)
            return;

        var angle = Vector3.SignedAngle(Vector3.forward, currentMoveVector, Vector3.up);
        var direction = DirectionUtility.FromAngle(angle);
        blockMovement.MoveInDirection(direction);
    }

}
