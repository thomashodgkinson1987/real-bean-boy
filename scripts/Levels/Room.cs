using Godot;

public partial class Room : Node2D
{
	[Export] public Vector2I Dimensions;

	public Vector2I GetBounds()
	{
		return Vector2I.One * Dimensions;
	}

	public virtual void OnEnter() { }
	public virtual void OnExit() { }
}
