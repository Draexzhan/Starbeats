using Godot;
using System;

public partial class SparkSpawner : Node2D
{
	[Export]
	public PackedScene SparkScene {get; set;}
	public PackedScene RayScene {get; set;}
	
	public void SpawnSparks() {
		for (int i = 0; i < 12; i++) {
			//creating new sparks
			//Spark newSpark = SparkScene.Instantiate<Sprite2D>();
			var sparkSprite = GetNode<Sprite2D>("Spark");
			
			//choosing their direction and speed
			float direction = (float)i * Mathf.Pi / 6 + (float)GD.RandRange(0.0, 2.0);
			Vector2 velocity = new Vector2((float)GD.RandRange(50.0, 300.0), 0);
			
			//assigning this vector
			//newSpark.LinearVelocity = velocity.Rotated(direction);
		}
	}
}
