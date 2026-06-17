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

		for (int i = 0; i < rooms.Count; i++)
		{
			Vector2 position = rooms[i].GlobalPosition;
			Vector2I dimensions = rooms[i].Dimensions;
			Rect2I rect = new Rect2I((int)position.X, (int)position.Y, dimensions.X * 128, dimensions.Y * 128);

			if (rect.HasPoint(beanBoy.GetCentreGlobalI()))
			{
				return rooms[i];
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
		Vector2 position = room.GlobalPosition;
		Vector2I dimensions = room.Dimensions;

		camera2D.LimitLeft = (int)position.X;
		camera2D.LimitRight = (int)position.X + dimensions.X * 128;
		camera2D.LimitTop = (int)position.Y;
		camera2D.LimitBottom = (int)position.Y + dimensions.Y * 128;
	}
}
