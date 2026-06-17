using System.Collections.Generic;
using Godot;

public partial class MainScene : Node2D
{
	[Export] private PackedScene beanBoyPackedScene;
	[Export] private PackedScene sproutValleyPackedScene;

	private Camera2D camera2D;
	private Node2D levelHolder;
	private Node2D entitiesHolder;

	private BeanBoy beanBoy;

	private Level currentLevel;
	private Room currentRoom;

	public override void _Ready()
	{
		// get nodes
		camera2D = GetNode<Camera2D>("Camera2D");
		levelHolder = GetNode<Node2D>("LevelHolder");
		entitiesHolder = GetNode<Node2D>("EntitiesHolder");

		// level
		currentLevel = sproutValleyPackedScene.Instantiate<Level>();
		levelHolder.AddChild(currentLevel);

		// bean boy
		beanBoy = beanBoyPackedScene.Instantiate<BeanBoy>();
		entitiesHolder.AddChild(beanBoy);
		beanBoy.GlobalPosition = currentLevel.GetSpawnPoint().GlobalPosition;
		beanBoy.GetCameraSensor().AreaEntered += OnAreaEnteredBeanBoy;

		// room
		currentRoom = GetCurrentRoom();

		// camera
		SetCameraBounds(currentRoom);
		camera2D.GlobalPosition = beanBoy.GetCentreGlobal();
		camera2D.ResetSmoothing();

		currentLevel.OnEnter();
		currentRoom.OnEnter();
	}

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.Escape))
			GetTree().Quit();

		if (Input.IsKeyPressed(Key.R))
		{
			GameState.Instance.ResetSession();
			GetTree().ReloadCurrentScene();
		}

		camera2D.GlobalPosition = beanBoy.GetCentreGlobal();
	}

	private Room GetCurrentRoom()
	{
		List<Room> rooms = currentLevel.GetRooms();

		foreach (Room room in rooms)
		{
			if (room.GetGlobalBoundsI().HasPoint(beanBoy.GetCentreGlobalI()))
			{
				return room;
			}
		}

		return null;
	}

	private void OnAreaEnteredBeanBoy(Area2D area)
	{
		UpdateCameraBounds();
	}

	private void UpdateCameraBounds()
	{
		Room newRoom = GetCurrentRoom();

		if (currentRoom != null && newRoom != currentRoom)
		{
			currentRoom.OnExit();
			currentRoom = newRoom;
			currentRoom.OnEnter();
			SetCameraBounds(currentRoom);
		}
	}

	private void SetCameraBounds(Room room)
	{
		Rect2I bounds = room.GetGlobalBoundsI();

		camera2D.LimitLeft = bounds.Position.X;
		camera2D.LimitRight = bounds.End.X;
		camera2D.LimitTop = bounds.Position.Y;
		camera2D.LimitBottom = bounds.End.Y;
	}
}
