using Godot;

public partial class MainScene : Node2D
{
	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.Escape))
			GetTree().Quit();
	}
}
