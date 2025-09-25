using Godot;
using System;
using System.Threading;


public partial class Star : Area2D
{
	public int LEADUP_SECONDS = 5;

	[Export] public string inputAction;

	[Signal] public delegate void StarClickedEventHandler();

	public override void _Ready()
	{
		if (inputAction == null)
		{
			GD.PrintErr("no event action");
		}
	}

	public void SpawnNote()
	{
		AnimationPlayer player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.Play("playNote");
	}

    public override void _Process(double delta)
    {
		if (Input.IsActionPressed(inputAction))
		{
			// calculate timing
			EmitSignal(SignalName.StarClicked);
		}
    }
}
