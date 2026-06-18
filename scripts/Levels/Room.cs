using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Room : Node2D
{
	[Export] public Vector2I Dimensions;

	private Area2D bounds;
	private CollisionShape2D boundsCollisionShape2D;
	private RectangleShape2D boundsRectangleShape2D;
	private Node2D checkpointsHolder;

	private List<Vector2> checkpoints;

	public override void _Ready()
	{
		bounds = GetNode<Area2D>("Bounds");
		boundsCollisionShape2D = bounds.GetNode<CollisionShape2D>("CollisionShape2D");
		boundsRectangleShape2D = (RectangleShape2D)boundsCollisionShape2D.Shape;

		bounds.Position = new Vector2(Dimensions.X * 128 / 2, Dimensions.Y * 128 / 2);
		boundsRectangleShape2D.Size = new Vector2((Dimensions.X * 128) - 4, (Dimensions.Y * 128) - 4);

		checkpointsHolder = GetNode<Node2D>("Checkpoints");
		checkpoints = new List<Vector2>();
		foreach (Marker2D checkpoint in checkpointsHolder.GetChildren().Cast<Marker2D>())
		{
			checkpoints.Add(checkpoint.GlobalPosition);
		}
	}

	public Area2D GetBoundsArea2D() => bounds;

	public Rect2I GetBoundsI()
	{
		return new Rect2I((int)Position.X, (int)Position.Y, Dimensions.X * 128, Dimensions.Y * 128);
	}

	public Rect2I GetGlobalBoundsI()
	{
		return new Rect2I((int)GlobalPosition.X, (int)GlobalPosition.Y, Dimensions.X * 128, Dimensions.Y * 128);
	}

	public List<Vector2> GetCheckpoints() => checkpoints;

	public virtual void OnEnter()
	{
		boundsCollisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
	public virtual void OnExit()
	{
		boundsCollisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
	}
}
