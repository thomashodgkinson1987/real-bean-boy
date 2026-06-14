using Godot;

public partial class PinkFlower : AnimatedSprite2D
{
	public override void _Ready()
	{
		Animation = "animate";

		int frameCount = SpriteFrames.GetFrameCount(Animation);
		int frame = GD.RandRange(0, frameCount);
		SetFrameAndProgress(frame, GD.Randf());

		Play();
	}
}
