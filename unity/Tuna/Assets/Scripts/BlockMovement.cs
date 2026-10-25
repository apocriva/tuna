using System;
using Unity.Mathematics;
using UnityEngine;

[Flags]
public enum PassageType
{
    None = 0,
    Walkable = 1 << 0,
}

public class BlockMovement : MonoBehaviour
{

    [field: SerializeField, Min(0f), Tooltip("In units of blocks/sec")]
    public float MoveSpeed { get; private set; }
    [field: SerializeField, Min(0f), Tooltip("In units of 90º/sec")]
    public float LookSpeed { get; private set; }

    public bool IsMoving => CurrentMoveAction != null;
    public bool IsLooking => CurrentLookAction != null;
    public int3 CurrentBlockPos => BlockVectorExtensions.FromWorld(transform.position);
    public WorldDirection CurrentFacing => DirectionUtility.WorldFromHeading(transform.rotation.eulerAngles.y);

    private MoveAction CurrentMoveAction { get; set; } = null;
    private LookAction CurrentLookAction { get; set; } = null;

    private class MoveAction
    {
        public int3 Start { get; private set; }
        public int3 End { get; private set; }
        public float StartTime { get; private set; }
        public float Speed { get; private set; }
        public float Progress { get; private set; }

        public MoveAction(int3 start, int3 end, float speed)
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

    private class LookAction
    {
        public float Start { get; private set; }
        public float End {get; private set; }
        public float StartTime { get; private set; }
        public float Speed { get; private set; }
        public float Progress { get; private set; }

        public LookAction(WorldDirection start, WorldDirection end, float speed)
        {
            Start = DirectionUtility.ToHeading(start);
            End = Start - Mathf.DeltaAngle(Start, DirectionUtility.ToHeading(end));
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

        public Quaternion CalculateRotation()
        {
            return Quaternion.Euler(0f, Start + (End - Start) * Progress, 0f);
        }
    }

    public void MoveInDirection(Direction direction)
    {
        if (IsMoving) return;

        // var tileData = Level.Current.Tilemap[CurrentBlockPos];

        var target = CurrentBlockPos + direction.RelativeTo(CurrentFacing).ToInt3();
        if (Level.Current.GetPassageBetween(CurrentBlockPos, target) == PassageType.Walkable)
        {
            CurrentMoveAction = new(
                CurrentBlockPos,
                target,
                MoveSpeed
            );
        }
    }

    public void TurnInDirection(Direction direction)
    {
        if (IsLooking) return;

        if (direction != Direction.Left && direction != Direction.Right)
            throw new ArgumentException();

        CurrentLookAction = new(
            CurrentFacing,
            direction.RelativeTo(CurrentFacing),
            LookSpeed
        );
    }

    public void Update()
    {
        UpdateMoveAction();
        UpdateLookAction();
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

    private void UpdateLookAction()
    {
        if (CurrentLookAction == null) return;

        CurrentLookAction.Update(Time.deltaTime, out var isComplete);
        transform.rotation = CurrentLookAction.CalculateRotation();
        if (isComplete)
        {
            CurrentLookAction = null;
        }
    }

}
