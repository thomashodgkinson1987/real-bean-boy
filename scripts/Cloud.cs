using Godot;

public partial class Cloud : Sprite2D
{

	public enum DirectionType
	{
		Left = -1,
		Right = 1
	}

	[Export] public DirectionType Direction = DirectionType.Left;
	[Export] public float Speed = 16.0f;
	[Export] public int LeftBounds = 0;
	[Export] public int RightBounds = 128;

	public override void _Ready()
	{
		Speed = (float)GD.RandRange(Speed - 2, Speed);
	}

	public override void _Process(double delta)
	{
		Vector2 position = Position;

		position.X += ((int)Direction) * Speed * (float)delta;

		if (position.X < LeftBounds - 8)
		{
			position.X = RightBounds;
		}
		else if (position.X > RightBounds)
		{
			position.X = LeftBounds - 8;
		}

		Position = position;
	}
}
