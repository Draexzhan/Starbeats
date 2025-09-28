using Godot;
using System;
using System.Linq;
using System.Threading;


public partial class Star : Area2D
{
	public int LEADUP_SECONDS = 5;

	private CharterPlayer _chartPlayer;
	private Globals globals;

	PackedScene arrow = GD.Load<PackedScene>("res://arrow.tscn");

	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
	}

	public void SpawnNote(string inputKey, float timing)
	{
		Arrow a = arrow.Instantiate<Arrow>();
		AddChild(a);
		a.SetDirection(inputKey);
		a.setHitTimer(timing);
	}

	public override void _Process(double delta)
	{
		if (globals.ActiveStar != this) return;

		string direction = null;
		if (Input.IsActionJustPressed("Up")) direction = "Up";
		else if (Input.IsActionJustPressed("Down")) direction = "Down";
		else if (Input.IsActionJustPressed("Left")) direction = "Left";
		else if (Input.IsActionJustPressed("Right")) direction = "Right";

		if (direction == null) return;
		Arrow arrow = GetNote(direction);

		if (arrow == null) return;
		GradeNoteHit(arrow);
	}

	Arrow GetNote(string direction)
	{
		var children = GetChildren();
		Arrow arrow = children.FirstOrDefault(child => child is Arrow arrow && arrow.direction == direction, null) as Arrow;
		return arrow;
	}

	void GradeNoteHit(Arrow arrow)
	{
		var rawGrade = arrow.hitTime - globals.audioTimer;
		var absGrade = Math.Abs(rawGrade);
		if (rawGrade < 0.5)
		{
			arrow.QueueFree();
		}
		GD.Print("HIT :: ", rawGrade < 0 ? "LATE" : "EARLY", absGrade);
	}
}
