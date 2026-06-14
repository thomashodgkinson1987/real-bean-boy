using System.Collections.Generic;
using Godot;

public partial class MainScene : Node2D
{
	private Camera2D camera2D;
	private Node2D levelHolder;
	private BeanBoy beanBoy;

	private Level level;
	private LevelSection levelSection;

	public override void _Ready()
	{
		camera2D = GetNode<Camera2D>("Camera2D");
		levelHolder = GetNode<Node2D>("LevelHolder");
		beanBoy = GetNode<BeanBoy>("BeanBoy");

		level = levelHolder.GetChild<Level>(0);

		beanBoy.GetCameraSensor().AreaEntered += OnAreaEnteredBeanBoy;

		UpdateCameraBounds();

		camera2D.Position = beanBoy.GetCentreGlobal();
		camera2D.ResetSmoothing();
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

		// Vector2 beanBoyPosition = beanBoy.GetCentreGlobal();
		// int beanBoyX = (int)beanBoyPosition.X;
		// int beanBoyY = (int)beanBoyPosition.Y;
		// int beanBoyCellX = beanBoyX / 128;
		// int beanBoyCellY = beanBoyY / 128;

		// camera2D.Position = new Vector2(64 + beanBoyCellX * 128, 64 + beanBoyCellY * 128);

		camera2D.Position = beanBoy.GetCentreGlobal();
	}

	private void OnAreaEnteredBeanBoy(Area2D area)
	{
		UpdateCameraBounds();
	}

	private void UpdateCameraBounds()
	{
		List<LevelSection> sections = level.GetSections();

		for (int i = 0; i < sections.Count; i++)
		{
			Vector2 position = sections[i].Position;
			Rect2I rect = sections[i].GetBounds();

			if (beanBoy.GetCentreGlobal().X > position.X + rect.Position.X &&
				beanBoy.GetCentreGlobal().X < position.X + rect.End.X &&
				beanBoy.GetCentreGlobal().Y > position.Y + rect.Position.Y &&
				beanBoy.GetCentreGlobal().Y < position.Y + rect.End.Y)
			{
				levelSection = sections[i];
				camera2D.LimitLeft = (int)position.X + rect.Position.X;
				camera2D.LimitRight = (int)position.X + rect.End.X;
				camera2D.LimitTop = (int)position.Y + rect.Position.Y;
				camera2D.LimitBottom = (int)position.Y + rect.End.Y;
				break;
			}
		}
	}
}
