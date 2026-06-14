using Godot;

public partial class BeanBoy : CharacterBody2D
{
	private AnimatedSprite2D animatedSprite2D;

	[Export] public float Speed = 48.0f;
	[Export] public float JumpVelocity = -64.0f;

	private float timer = 0.0f;
	private float timeLimit = 1.0f;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("player_jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("player_left", "player_right", "ui_up", "ui_down");
		direction.Y = 0.0f;

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		//

		animatedSprite2D.FlipH = Velocity.X < 0 || (Velocity.X <= 0 && animatedSprite2D.FlipH);

		if (!IsOnFloor())
		{
			timer = 0.0f;
			animatedSprite2D.Animation = "jump";
		}
		else if (Velocity.X != 0)
		{
			timer = 0.0f;
			animatedSprite2D.Animation = "walk";
		}
		else
		{
			if (animatedSprite2D.Animation != "idle")
			{
				timer += (float)delta;
				if (timer < timeLimit)
				{
					animatedSprite2D.Animation = "default";
				}
				else
				{
					animatedSprite2D.Animation = "idle";
				}
			}
		}
	}
}
