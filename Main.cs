using Godot;
using System;

public partial class Main : Node
{
	//This enum dictates what we're currently doing in the game.
	public enum GameState
	{
		StarSelect = 0, //We have not yet picked a star
		Rhythm = 1 //We are doing the rhythm game
	};
	public int CurrentState;
	
	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		CurrentState = (int)GameState.StarSelect;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Confirm") && CurrentState == (int)GameState.StarSelect)
		{
			//TODO: Center camera on selected star, potentially make everything else leave the screen like you're zooming in.
			GD.Print("Click!");
			CurrentState = (int)GameState.Rhythm; 
		}
		
		//We will only check for these inputs during the rhythm game.
		if (CurrentState == (int)GameState.Rhythm)
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
}
