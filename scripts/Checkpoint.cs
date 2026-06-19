using Godot;


public partial class Checkpoint : Area2D
{
	private Marker2D spawnPoint;
	private AnimatedSprite2D animatedSprite2D;

	public CheckpointState State;

	public override void _Ready()
	{
		spawnPoint = GetNode<Marker2D>("SpawnPoint");
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		State = CheckpointState.Lowered;
	}

	public Vector2 GetSpawnPoint() => spawnPoint.Position;
	public Vector2 GetSpawnPointGlobal() => spawnPoint.GlobalPosition;

	public void Reset()
	{
		animatedSprite2D.Play("default");
		State = CheckpointState.Lowered;
	}

	public void Raise()
	{
		animatedSprite2D.Play("raise");
		State = CheckpointState.Raised;
	}

	public void Lower()
	{
		animatedSprite2D.Play("lower");
		State = CheckpointState.Lowered;
	}
}
