using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Level : Node2D
{
	protected Marker2D spawnPoint;
	protected Node2D roomsHolder;

	protected List<Room> rooms;

	public override void _Ready()
	{
		spawnPoint = GetNode<Marker2D>("SpawnPoint");
		roomsHolder = GetNode<Node2D>("Rooms");

		rooms = new List<Room>();
		rooms.AddRange(roomsHolder.GetChildren().Cast<Room>());
	}

	public Marker2D GetSpawnPoint() => spawnPoint;

	public List<Checkpoint> GetCheckpoints()
	{
		List<Checkpoint> checkpoints = new List<Checkpoint>();

		foreach (Room room in rooms)
		{
			foreach (Checkpoint checkpoint in room.GetCheckpoints())
			{
				checkpoints.Add(checkpoint);
			}
		}

		return checkpoints;
	}

	public List<Room> GetRooms() => rooms;

	public virtual void OnEnter() { }
	public virtual void OnExit() { }
}
