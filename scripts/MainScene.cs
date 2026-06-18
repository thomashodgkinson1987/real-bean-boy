using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class MainScene : Node2D
{
	[Export] private PackedScene beanBoyPackedScene;
	[Export] private PackedScene sproutValleyPackedScene;

	private CameraController camera2D;
	private Node2D levelHolder;
	private Node2D entitiesHolder;

	private BeanBoy beanBoy;

	private Level currentLevel;
	private Room currentRoom;
	private Vector2 currentCheckpoint;

	public override void _Ready()
	{
		// get nodes
		camera2D = GetNode<CameraController>("Camera2D");
		levelHolder = GetNode<Node2D>("LevelHolder");
		entitiesHolder = GetNode<Node2D>("EntitiesHolder");

		// level
		currentLevel = sproutValleyPackedScene.Instantiate<Level>();
		levelHolder.AddChild(currentLevel);

		// bean boy
		beanBoy = beanBoyPackedScene.Instantiate<BeanBoy>();
		entitiesHolder.AddChild(beanBoy);
		beanBoy.GlobalPosition = currentLevel.GetSpawnPoint().GlobalPosition;
		beanBoy.GetRoomTransitionSensor().AreaEntered += OnAreaEnteredBeanBoy;
		beanBoy.GetHitBox().AreaEntered += OnAreaEntered_BeanBoy_HitBox;

		// room
		currentRoom = GetCurrentRoom();

		// checkpoint
		currentCheckpoint = currentLevel.GetSpawnPoint().GlobalPosition;

		// camera
		camera2D.SetLimits(currentRoom);
		camera2D.GlobalPosition = beanBoy.GetCentreGlobal();
		camera2D.ResetSmoothing();
		camera2D.SetMode(CameraMode.Target);
		camera2D.SetTarget(beanBoy.CentreMarker);

		// enter level and room
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
		_ = CheckIfSceneTransition();
	}

	private void OnAreaEntered_BeanBoy_HitBox(Area2D area)
	{
		beanBoy.GlobalPosition = currentCheckpoint;
	}

	private async Task CheckIfSceneTransition()
	{
		if (GetCurrentRoom() is Room newRoom && newRoom != currentRoom)
		{
			beanBoy.CallDeferred(GodotObject.MethodName.Set, Node.PropertyName.ProcessMode, (int)ProcessModeEnum.Disabled);
			currentLevel.ProcessMode = ProcessModeEnum.Disabled;

			currentRoom.OnExit();
			currentRoom = newRoom;
			currentRoom.OnEnter();

			currentCheckpoint = currentRoom.GetCheckpoints()[0];
			foreach (Vector2 checkpoint in currentRoom.GetCheckpoints())
			{
				if (beanBoy.GlobalPosition.DistanceSquaredTo(checkpoint) < beanBoy.GlobalPosition.DistanceSquaredTo(currentCheckpoint))
				{
					currentCheckpoint = checkpoint;
				}
			}

			camera2D.SetLimits(currentRoom);

			camera2D.GlobalPosition = camera2D.GetScreenCenterPosition();
			camera2D.ResetPhysicsInterpolation();

			camera2D.LimitEnabled = false;
			camera2D.LimitSmoothed = false;
			camera2D.PositionSmoothingEnabled = false;

			camera2D.SetMode(CameraMode.Transition);
			await ToSignal(camera2D, "MovementFinished");
			camera2D.SetMode(CameraMode.Target);

			camera2D.LimitEnabled = true;
			camera2D.LimitSmoothed = true;
			camera2D.PositionSmoothingEnabled = true;

			beanBoy.CallDeferred(GodotObject.MethodName.Set, Node.PropertyName.ProcessMode, (int)ProcessModeEnum.Inherit);
			currentLevel.ProcessMode = ProcessModeEnum.Inherit;
		}
	}


}
