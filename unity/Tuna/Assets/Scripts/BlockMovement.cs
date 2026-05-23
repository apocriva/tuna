using UnityEngine;

public class BlockMovement : MonoBehaviour
{

    [field: SerializeField, Min(0f)]
    public float MoveSpeed { get; private set; }

    public bool IsMoving => CurrentMoveAction != null;
    public BlockVector CurrentBlockPos => BlockVector.FromWorld(transform.position);
    public WorldDirection CurrentFacing => DirectionUtility.WorldFromHeading(transform.rotation.y);

    private MoveAction CurrentMoveAction { get; set; } = null;

    private class MoveAction
    {
        public BlockVector Start { get; private set; }
        public BlockVector End { get; private set; }
        public float StartTime { get; private set; }
        public float Speed { get; set; }
        public float Progress { get; set; }

        public MoveAction(BlockVector start, BlockVector end, float speed)
        {
            Start = start;
            End = end;
            StartTime = Time.time;
            Speed = speed;
            Progress = 0f;
        }

        public void Update(float deltaTime, out bool isComplete)
        {
            Progress += deltaTime * Speed;
            if (Progress >= 1f)
            {
                Progress = 1f;
                isComplete = true;
            }
            else
            {
                isComplete = false;
            }
        }

        public Vector3 CalculateWorldPosition()
        {
            return Vector3.Lerp(Start.ToWorld(), End.ToWorld(), Progress);
        }
    }

    public void MoveInDirection(Direction direction)
    {
        if (IsMoving) return;

        CurrentMoveAction = new MoveAction(
            CurrentBlockPos,
            CurrentBlockPos + direction.RelativeTo(CurrentFacing).ToBlockVector(),
            MoveSpeed
        );
    }

    public void Update()
    {
        UpdateMoveAction();
    }

    private void UpdateMoveAction()
    {
        if (CurrentMoveAction == null) return;

        CurrentMoveAction.Update(Time.deltaTime, out var isComplete);
        transform.position = CurrentMoveAction.CalculateWorldPosition();
        if (isComplete)
        {
            CurrentMoveAction = null;
        }
    }

}
