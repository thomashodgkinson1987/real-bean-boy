using Godot;

public partial class GameState : Node
{
	public static GameState Instance { get; private set; }

	[Export] public int GreenBeansCollected;

	public override void _Ready()
	{
		Instance = this;
	}

	public void CollectGreenBean()
	{
		GreenBeansCollected++;
		GD.Print($"GameState: CollectGreenBean: {GreenBeansCollected}");
	}
}
