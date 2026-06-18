using System.Collections.Generic;
using System.Linq;
using Godot;

public struct SpikeBallData
{
	public Vector2 Position;
	public float Rotation;
	public AxisX Direction;
	public int DirectionIndex;
}

public partial class Room : Node2D
{
	[Export] public Vector2I Dimensions;

	// nodes [start]

	private Area2D bounds;
	private CollisionShape2D boundsCollisionShape2D;
	private RectangleShape2D boundsRectangleShape2D;
	private Node2D checkpointsHolder;

	private Node2D spikeBallsHolder;

	// nodes [end]

	private List<Vector2> checkpoints;
	private List<SpikeBall> spikeBalls;
	private List<SpikeBallData> spikeBallsDefaultData;

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

		spikeBallsHolder = GetNode<Node2D>("SpikeBalls");
		spikeBalls = new List<SpikeBall>();
		spikeBalls.AddRange(spikeBallsHolder.GetChildren().Cast<SpikeBall>());
		spikeBallsDefaultData = new List<SpikeBallData>();
		foreach (SpikeBall spikeBall in spikeBalls)
		{
			SpikeBallData data;
			data.Position = spikeBall.GlobalPosition;
			data.Rotation = spikeBall.Rotation;
			data.Direction = spikeBall.Direction;
			data.DirectionIndex = spikeBall.DirectionIndex;
			spikeBallsDefaultData.Add(data);
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

	public void Reset()
	{
		for (int i = 0; i < spikeBalls.Count; i++)
		{
			SpikeBallData data = spikeBallsDefaultData[i];

			spikeBalls[i].GlobalPosition = data.Position;
			spikeBalls[i].Rotation = data.Rotation;
			spikeBalls[i].Direction = data.Direction;
			spikeBalls[i].DirectionIndex = data.DirectionIndex;

			spikeBalls[i].SetProcess(true);
			spikeBalls[i].SetPhysicsProcess(true);
			spikeBalls[i].ProcessMode = ProcessModeEnum.Inherit;
		}
	}

	public virtual void OnEnter()
	{
		boundsCollisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		for (int i = 0; i < spikeBalls.Count; i++)
		{
			SpikeBallData data = spikeBallsDefaultData[i];

			spikeBalls[i].GlobalPosition = data.Position;
			spikeBalls[i].Rotation = data.Rotation;
			spikeBalls[i].Direction = data.Direction;
			spikeBalls[i].DirectionIndex = data.DirectionIndex;

			spikeBalls[i].ProcessMode = ProcessModeEnum.Disabled;
			spikeBalls[i].SetProcess(false);
			spikeBalls[i].SetPhysicsProcess(false);
		}
	}
	public virtual void OnEnterTransitionFinished()
	{
		for (int i = 0; i < spikeBalls.Count; i++)
		{
			spikeBalls[i].SetProcess(true);
			spikeBalls[i].SetPhysicsProcess(true);
			spikeBalls[i].ProcessMode = ProcessModeEnum.Inherit;
		}
	}
	public virtual void OnExit()
	{
		boundsCollisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		for (int i = 0; i < spikeBalls.Count; i++)
		{
			spikeBalls[i].SetProcess(false);
			spikeBalls[i].SetPhysicsProcess(false);
			spikeBalls[i].ProcessMode = ProcessModeEnum.Disabled;
		}
	}
}
