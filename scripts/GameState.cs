using Godot;

public partial class GameState : Node
{
	public static GameState Instance { get; private set; }

	[Export] public int GreenBeansCollected;
	[Export] public int GoldenBeansCollected;

	public override void _Ready()
	{
		Instance = this;
	}

	public void CollectGreenBean()
	{
		GreenBeansCollected++;
		GD.Print($"GameState: CollectGreenBean: {GreenBeansCollected}");
	}

	public void CollectGoldenBean()
	{
		GoldenBeansCollected++;
		GD.Print($"GameState: CollectGoldenBean: {GoldenBeansCollected}");
	}
}
