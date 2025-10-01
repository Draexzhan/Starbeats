using Godot;
using System;

public partial class SparkSpawner : Node2D
{
	[Export]
	public PackedScene SparkScene { get; set; }
	
	[Export]
	public PackedScene RayScene { get; set; }
	
	public float heatScale;

	public override void _Ready()
	{
		base._Ready();
		GetNode<Sprite2D>("Sun").Scale = new Vector2(0.4f, 0.4f);
		SpawnRays();
		Scale = new Vector2(0.4f, 0.4f);
	}

	public void SpawnRays()
	{
		for (int i = 0; i < 3; i++)
		{
			//Creating new rays
			SunRay newRay = RayScene.Instantiate<SunRay>();

			//choose rotation direction and speed
			float rotation = (float)GD.RandRange(0.0, 1.0) - 1.5f + i;
			newRay.rotSpeed = rotation;
			AddChild(newRay);
		}
	}

	public void SpawnSparks(int quantity)
	{
		for (int i = 0; i < quantity; i++)
		{
			//creating new sparks
			Spark newSpark = SparkScene.Instantiate<Spark>();

			//choosing their direction and speed
			float direction = i * Mathf.Pi / (quantity / 2.0f) + (float)GD.RandRange(0.0, 2.0);
			Vector2 velocity = new Vector2((float)GD.RandRange(0.1, 2.0), 0);

			//assigning this vector
			newSpark.LinearVelocity = velocity.Rotated(direction);
			AddChild(newSpark);
		}
		Scale += new Vector2((float)quantity / 1000, (float)quantity / 1000);
		((ShaderMaterial)Material).SetShaderParameter("heat", Scale.X - 0.4);
		GD.Print(Scale.X.ToString());
	}
}
