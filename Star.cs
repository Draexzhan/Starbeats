using Godot;
using System;
using System.Linq;
using System.Threading;


public partial class Star : Area2D
{
	private CharterPlayer _chartPlayer;
	private Globals globals;
	private Label label;

	PackedScene arrow = GD.Load<PackedScene>("res://arrow.tscn");

	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
		label = GetNode<Label>("Control/Label");

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
		GD.Print("CHILD COUNT: ", GetChildCount());
		Arrow arrow = children.FirstOrDefault(child => child is Arrow arrow && arrow.direction == direction, null) as Arrow;
		return arrow;
	}

	void GradeNoteHit(Arrow arrow)
	{
		var rawGrade = arrow.hitTime - globals.audioTimer;

		if (rawGrade < 0.5)
		{
			label.Text = DisplayGrade(rawGrade);
			arrow.Hit();
		}
		else
		{
			label.Text = "";
		}
	}

	string DisplayGrade(float rawGrade)
	{
		var absGrade = Math.Abs(rawGrade);
		var msOff = Math.Round(absGrade * 1000);

		var grade = msOff switch
		{
			< 30 => "PERFECT",
			< 60 => "GOOD",
			< 120 => "OK",
			_ => "BAD",
		};
		var lateOrEarly = rawGrade < 0 ? "late" : "early";
		var guide = $"{lateOrEarly} {msOff}ms";
		return $"{grade} ({guide})";
	}
}
