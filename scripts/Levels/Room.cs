using Godot;

public partial class Room : Node2D
{
	[Export] public Vector2I Dimensions;

	public Rect2I GetBoundsI()
	{
		return new Rect2I((int)Position.X, (int)Position.Y, Dimensions.X * 128, Dimensions.Y * 128);
	}

	public Rect2I GetGlobalBoundsI()
	{
		return new Rect2I((int)GlobalPosition.X, (int)GlobalPosition.Y, Dimensions.X * 128, Dimensions.Y * 128);
	}

	public virtual void OnEnter() { }
	public virtual void OnExit() { }
}
