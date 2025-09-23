using Godot;
using System;

public partial class Main : Node
{
	[Export] public PackedScene Star {get; set;}
	
	
	//This enum dictates what we're currently doing in the game.
	public enum GameState
	{
		StarSelect = 0, //We have not yet picked a star
		Rhythm = 1 //We are doing the rhythm game
	};
	public int currentState;
	public Vector2 cameraViewpoint = Vector2.Zero;
	private Vector2 cameraDestination = Vector2.Zero;
	
	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		currentState = (int)GameState.StarSelect;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton)
		{
			//TODO: Center camera on selected star, potentially make everything else leave the screen like you're zooming in.
			GD.Print("Click!");
			currentState = (int)GameState.Rhythm; 
		}
		
		//We will only check for these inputs during the rhythm game.
		if (currentState == (int)GameState.Rhythm)
		{
			if (Input.IsActionJustPressed("Up"))
			{
				GD.Print("Up!");
			}
			if (Input.IsActionJustPressed("Down"))
			{
				GD.Print("Down!");
			}
			if (Input.IsActionJustPressed("Left"))
			{
				GD.Print("Left!");
			}
			if (Input.IsActionJustPressed("Right"))
			{
				GD.Print("Right!");
			}
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
