using Godot;
using System;

public partial class SparkSpawner : Node2D
{
	[Export]
	public PackedScene SparkScene { get; set; }
	public PackedScene RayScene { get; set; }
	private float heatScale;

	public override void _Ready()
	{
		base._Ready();
		//SpawnRays();
		GetNode<Sprite2D>("Sun").Scale = new Vector2(0.4f, 0.4f);
	}

	public void SpawnRays()
	{
		for (int i = 0; i < 3; i++)
		{
			//Creating new rays
			Ray newRay = (Ray)RayScene.Instantiate<Sprite2D>();

			//choose rotation direction and speed
			float rotation = (float)GD.RandRange(0.0, 1.0) - 1.5f + i;
			newRay.rotSpeed = rotation;
			AddChild(newRay);
		}
	}

	public void SpawnSparks()
	{
		for (int i = 0; i < 12; i++)
		{
			//creating new sparks
			Spark newSpark = (Spark)SparkScene.Instantiate<Sprite2D>();

			//choosing their direction and speed
			float direction = i * Mathf.Pi / 6 + (float)GD.RandRange(0.0, 2.0);
			Vector2 velocity = new Vector2((float)GD.RandRange(50.0, 300.0), 0);

			//assigning this vector
			newSpark.LinearVelocity = velocity.Rotated(direction);
			AddChild(newSpark);
		}
	}
	public override void _Process(double delta)
	{
		Scale += new Vector2((float)delta/6, (float)delta/6);
	}
}
