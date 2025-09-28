using Godot;
using System;

public partial class Spark : Sprite2D
{
	public Vector2 LinearVelocity;
	public override void _Process(double delta)
	{
		base._Process(delta);
		Position += LinearVelocity;
		Scale = new Vector2(Mathf.Clamp(Scale.X - 0.2f * (float)delta, 0, 1), Mathf.Clamp(Scale.X - 0.2f * (float)delta, 0, 1));
		if (Scale.X < 0.001)
			QueueFree();
	}
}
