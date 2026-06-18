using Godot;

public partial class SpikeBall : Area2D
{
	[Export] public int StepDelay = 2;
	[Export] public AxisX Direction = AxisX.Right;
	[Export] public int DirectionIndex = 0;

	private AnimatedSprite2D animatedSprite2D;
	private RayCast2D leftRayCast2D;
	private RayCast2D rightRayCast2D;
	private RayCast2D frontRayCast2D;
	private RayCast2D backRayCast2D;

	private int frameCounter;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		leftRayCast2D = GetNode<RayCast2D>("LeftRayCast2D");
		rightRayCast2D = GetNode<RayCast2D>("RightRayCast2D");
		frontRayCast2D = GetNode<RayCast2D>("FrontRayCast2D");
		backRayCast2D = GetNode<RayCast2D>("BackRayCast2D");

		DirectionIndex = Mathf.Clamp(DirectionIndex, 0, 3);
		RotationDegrees = DirectionIndex * 90;
		animatedSprite2D.RotationDegrees = DirectionIndex * 90;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (++frameCounter < StepDelay)
			return;
		frameCounter = 0;

		switch (Direction)
		{
			case AxisX.Left:
				LeftTick();
				break;
			case AxisX.Right:
				RightTick();
				break;
			default:
				break;
		}
	}

	private void LeftTick()
	{
		if ((leftRayCast2D.IsColliding() || rightRayCast2D.IsColliding()) && !backRayCast2D.IsColliding())
		{
			MoveBackward();
		}
		else if (!rightRayCast2D.IsColliding())
		{
			RotateCounterClockwise();
			MoveBackward();
		}
		else if (leftRayCast2D.IsColliding() && rightRayCast2D.IsColliding() && backRayCast2D.IsColliding())
		{
			RotateClockwise();
			leftRayCast2D.ForceRaycastUpdate();
			rightRayCast2D.ForceRaycastUpdate();
			backRayCast2D.ForceRaycastUpdate();
			if (!backRayCast2D.IsColliding())
			{
				MoveBackward();
			}
			else
			{
				RotateClockwise();
				MoveBackward();
			}
		}
	}

	private void RightTick()
	{
		if ((leftRayCast2D.IsColliding() || rightRayCast2D.IsColliding()) && !frontRayCast2D.IsColliding())
		{
			MoveForward();
		}
		else if (!leftRayCast2D.IsColliding())
		{
			RotateClockwise();
			MoveForward();
		}
		else if (leftRayCast2D.IsColliding() && rightRayCast2D.IsColliding() && frontRayCast2D.IsColliding())
		{
			RotateCounterClockwise();
			leftRayCast2D.ForceRaycastUpdate();
			rightRayCast2D.ForceRaycastUpdate();
			frontRayCast2D.ForceRaycastUpdate();
			if (!frontRayCast2D.IsColliding())
			{
				MoveForward();
			}
			else
			{
				RotateCounterClockwise();
				MoveForward();
			}
		}
	}

	private void RotateClockwise()
	{
		DirectionIndex = (DirectionIndex + 1) % 4;
		RotationDegrees = DirectionIndex * 90;
		animatedSprite2D.RotationDegrees = (DirectionIndex + 3) % 4 * 90;
	}

	private void RotateCounterClockwise()
	{
		DirectionIndex = (DirectionIndex + 3) % 4;
		RotationDegrees = DirectionIndex * 90;
		animatedSprite2D.RotationDegrees = (DirectionIndex + 1) % 4 * 90;
	}

	private void MoveForward()
	{
		Position += Transform.X;
	}

	private void MoveBackward()
	{
		Position += Transform.X * -1;
	}

	public void Reset()
	{
		frameCounter = 0;
		animatedSprite2D.Frame = 0;
	}

	public void ReCast()
	{
		leftRayCast2D.ForceRaycastUpdate();
		rightRayCast2D.ForceRaycastUpdate();
		frontRayCast2D.ForceRaycastUpdate();
		backRayCast2D.ForceRaycastUpdate();
	}

	public SpikeBallData GetState()
	{
		SpikeBallData state;

		state.Position = Position;
		state.Rotation = Rotation;
		state.Direction = Direction;
		state.DirectionIndex = DirectionIndex;

		return state;
	}

	public void SetState(SpikeBallData state)
	{
		Position = state.Position;
		Rotation = state.Rotation;
		Direction = state.Direction;
		DirectionIndex = state.DirectionIndex;
	}
}
