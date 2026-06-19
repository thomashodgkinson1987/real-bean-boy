using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class MainScene : Node2D
{
	[Export] private PackedScene beanBoyPackedScene;
	[Export] private PackedScene sproutValleyPackedScene;

	// nodes [start]

	private CameraController camera2D;
	private Node2D levelHolder;
	private Node2D entitiesHolder;

	private CanvasLayer faderCanvasLayer;
	private Control control;
	private TextureRect faderTextureRect;
	private AnimationPlayer faderAnimationPlayer;

	// nodes [end]

	private BeanBoy beanBoy;

	private Level currentLevel;
	private Room currentRoom;
	private Checkpoint currentCheckpoint;

	public override void _Ready()
	{
		// get nodes
		camera2D = GetNode<CameraController>("Camera2D");
		levelHolder = GetNode<Node2D>("LevelHolder");
		entitiesHolder = GetNode<Node2D>("EntitiesHolder");

		faderCanvasLayer = GetNode<CanvasLayer>("CanvasLayer");
		control = faderCanvasLayer.GetNode<Control>("Control");
		faderTextureRect = control.GetNode<TextureRect>("Fader");
		faderAnimationPlayer = control.GetNode<AnimationPlayer>("AnimationPlayer");

		// level
		currentLevel = sproutValleyPackedScene.Instantiate<Level>();
		levelHolder.AddChild(currentLevel);

		// bean boy
		beanBoy = beanBoyPackedScene.Instantiate<BeanBoy>();
		entitiesHolder.AddChild(beanBoy);
		beanBoy.GlobalPosition = currentLevel.GetSpawnPoint().GlobalPosition - Vector2.One * 4;
		beanBoy.GetRoomTransitionSensor().AreaEntered += OnAreaEntered_BeanBoy_RoomTransitionSensor;
		beanBoy.GetHitBox().AreaEntered += OnAreaEntered_BeanBoy_HitBox;

		// room
		currentRoom = GetCurrentRoom();

		// checkpoint
		currentCheckpoint = null;

		// camera
		camera2D.SetLimits(currentRoom);
		camera2D.GlobalPosition = beanBoy.GetCentreGlobal();
		camera2D.ResetSmoothing();
		camera2D.SetMode(CameraMode.Target);
		camera2D.SetTarget(beanBoy.CentreMarker);

		// enter level and room
		currentLevel.OnEnter();
		currentRoom.OnEnter();
		currentRoom.OnEnterTransitionFinished();
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

	private void OnAreaEntered_BeanBoy_RoomTransitionSensor(Area2D area)
	{
		_ = CheckIfSceneTransition();
	}

	private async Task CheckIfSceneTransition()
	{
		if (GetCurrentRoom() is Room newRoom && newRoom != currentRoom)
		{
			beanBoy.CallDeferred(GodotObject.MethodName.Set, Node.PropertyName.ProcessMode, (int)ProcessModeEnum.Disabled);
			//currentLevel.ProcessMode = ProcessModeEnum.Disabled;

			currentRoom.OnExit();
			currentRoom = newRoom;
			currentRoom.OnEnter();

			// currentCheckpoint = currentRoom.GetCheckpoints()[0];
			// foreach (Checkpoint checkpoint in currentRoom.GetCheckpoints())
			// {
			// 	if (beanBoy.GlobalPosition.DistanceSquaredTo(checkpoint.GetSpawnPointGlobal()) < beanBoy.GlobalPosition.DistanceSquaredTo(currentCheckpoint.GetSpawnPointGlobal()))
			// 	{
			// 		currentCheckpoint = checkpoint;
			// 	}
			// }

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
			//currentLevel.ProcessMode = ProcessModeEnum.Inherit;

			currentRoom.OnEnterTransitionFinished();
		}
	}

	private void OnAreaEntered_BeanBoy_HitBox(Area2D area)
	{
		if (area is Checkpoint checkpoint)
		{
			foreach (Checkpoint c in currentLevel.GetCheckpoints())
			{
				if (c != checkpoint && c.State == CheckpointState.Raised)
				{
					c.Lower();
				}
			}
			if (checkpoint != currentCheckpoint)
			{
				checkpoint.Raise();
				currentCheckpoint = checkpoint;
			}
		}
		else
		{
			_ = OnBeanBoyHit();
		}
	}

	private async Task OnBeanBoyHit()
	{
		beanBoy.SetProcess(false);
		beanBoy.SetPhysicsProcess(false);
		beanBoy.CallDeferred(GodotObject.MethodName.Set, Node.PropertyName.ProcessMode, (int)ProcessModeEnum.Disabled);

		currentRoom.Pause();

		faderAnimationPlayer.Play("fade_to_opaque");
		await ToSignal(faderAnimationPlayer, "animation_finished");

		if (currentCheckpoint == null)
		{
			beanBoy.GlobalPosition = currentLevel.GetSpawnPoint().GlobalPosition - Vector2.One * 4;
		}
		else
		{
			beanBoy.GlobalPosition = currentCheckpoint.GetSpawnPointGlobal() - Vector2.One * 4;
		}

		beanBoy.Reset();

		currentRoom.OnExit();
		currentRoom = GetCurrentRoom();
		currentRoom.OnEnter();

		camera2D.SetLimits(currentRoom);
		camera2D.GlobalPosition = beanBoy.GetCentreGlobal();
		camera2D.ResetSmoothing();

		faderAnimationPlayer.Play("fade_to_transparent");
		await ToSignal(faderAnimationPlayer, "animation_finished");

		beanBoy.SetProcess(true);
		beanBoy.SetPhysicsProcess(true);
		beanBoy.CallDeferred(GodotObject.MethodName.Set, Node.PropertyName.ProcessMode, (int)ProcessModeEnum.Inherit);

		currentRoom.OnEnterTransitionFinished();
	}

}
