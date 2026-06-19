using Godot;

public partial class CameraController : Camera2D
{
	[Signal]
	public delegate void MovementFinishedEventHandler();

	private Node2D target;
	private CameraMode mode;
	private Vector2 lastPosition;

	public override void _Ready()
	{
		lastPosition = GlobalPosition;
	}

	public override void _Process(double delta)
	{
		switch (mode)
		{
			case CameraMode.Target:
				TickTargetMode();
				break;
			case CameraMode.Transition:
				TickTransitionMode(delta);
				break;
		}
	}

	private void TickTargetMode()
	{
		if (target == null) return;
		GlobalPosition = target.GlobalPosition;
	}

	private void TickTransitionMode(double delta)
	{
		Rect2 destinationRect = new Rect2(target.GlobalPosition, 128, 128);

		if (destinationRect.Position.X < LimitLeft)
			destinationRect.Position = new Vector2(LimitLeft, destinationRect.Position.Y);
		if (destinationRect.End.X > LimitRight)
			destinationRect.Position = new Vector2(LimitRight - 128, destinationRect.Position.Y);
		if (destinationRect.Position.Y < LimitTop)
			destinationRect.Position = new Vector2(destinationRect.Position.X, LimitTop);
		if (destinationRect.End.Y > LimitBottom)
			destinationRect.Position = new Vector2(destinationRect.Position.X, LimitBottom - 128);

		Vector2 destination = destinationRect.GetCenter();
		float weight = Mathf.Pow((float)delta, 0.5f);
		GlobalPosition = GlobalPosition.Lerp(destination, weight);

		if ((destination - GetScreenCenterPosition()).Length() < 1.0f)
		{
			EmitSignal(SignalName.MovementFinished);
		}
	}

	public void SetTarget(Node2D newTarget)
	{
		target = newTarget;
	}

	public void SetMode(CameraMode newMode)
	{
		mode = newMode;
	}

	public void SetLimits(Room room)
	{
		Rect2I limitsRect = room.GetGlobalBoundsI();

		LimitLeft = limitsRect.Position.X;
		LimitRight = limitsRect.End.X;
		LimitTop = limitsRect.Position.Y;
		LimitBottom = limitsRect.End.Y;
	}
}
