using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 100f;
	private AnimatedSprite2D _anim;
	private Vector2 _input;
	private Vector2 _lastDirection = Vector2.Down;

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("Anim");
	}

	public override void _PhysicsProcess(double delta)
	{
		_input = Vector2.Zero;
		if (Input.IsActionPressed("move_up")) _input.Y -= 1;
		if (Input.IsActionPressed("move_down")) _input.Y += 1;
		if (Input.IsActionPressed("move_left")) _input.X -= 1;
		if (Input.IsActionPressed("move_right")) _input.X += 1;

		_input = _input.Normalized();

		Velocity = _input * Speed;
		MoveAndSlide();

		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		if (_input == Vector2.Zero)
		{
			PlayAnim("idle", _lastDirection);
			return;
		}

		_lastDirection = DominantDirection(_input);
		PlayAnim("walk", _lastDirection);
	}

	private Vector2 DominantDirection(Vector2 v)
	{
		if (Mathf.Abs(v.X) > Mathf.Abs(v.Y))
			return v.X > 0 ? Vector2.Right : Vector2.Left;
		return v.Y > 0 ? Vector2.Down : Vector2.Up;
	}

	private void PlayAnim(string type, Vector2 dir)
	{
		string anim = $"{type}_down";

		if (dir == Vector2.Up) anim = $"{type}_up";
		else if (dir == Vector2.Left) anim = $"{type}_left";
		else if (dir == Vector2.Right) anim = $"{type}_right";

		if (_anim.Animation != anim)
			_anim.Play(anim);
	}
}
