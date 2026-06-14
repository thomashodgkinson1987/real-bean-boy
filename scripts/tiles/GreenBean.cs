using Godot;

public partial class GreenBean : Area2D
{
	private AnimatedSprite2D animatedSprite2D;
	private AudioStreamPlayer audioStreamPlayer;

    public override void _Ready()
    {
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		BodyEntered += OnBodyEntered;
		audioStreamPlayer.Finished += OnFinished;
    }

	private void OnBodyEntered(Node2D body)
	{
		GD.Print("GreenBean: OnBodyEntered");
		BodyEntered -= OnBodyEntered;
		animatedSprite2D.Visible = false;
		audioStreamPlayer.Play();
		GameState.Instance.CollectGreenBean();
	}

	private void OnFinished()
	{
		GD.Print("GreenBean: OnFinished");
		audioStreamPlayer.Finished -= OnFinished;
		QueueFree();
	}
}
