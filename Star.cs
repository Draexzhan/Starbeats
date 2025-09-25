using Godot;
using System;
using System.Threading;


public partial class Star : Area2D
{
	public int LEADUP_SECONDS = 5;

	private HitKeyEmitter _emitterNode;
	private ChartPlayer _chartPlayer;
	private Globals globals;

	PackedScene arrow = GD.Load<PackedScene>("res://arrow.tscn");

	public override void _Ready()
	{
		globals = GetNode<Globals>("/root/Globals");
		_emitterNode = GetNode<HitKeyEmitter>("/root/Main/HitKeyEmitter");
	}

	public void SpawnNote(string inputKey)
	{
		Arrow a = arrow.Instantiate<Arrow>();
		AddChild(a);
		a.SetDirection(inputKey);
	}

    public override void _Process(double delta)
    {
		if (globals.ActiveStar != this) return;

		if (Input.IsActionJustPressed("Up"))
		{
			_emitterNode.Emit("Up", globals.ChartTimer);
		}
		else if (Input.IsActionJustPressed("Down"))
		{
			_emitterNode.Emit("Down", globals.ChartTimer);
		}
		else if (Input.IsActionJustPressed("Left"))
		{
			_emitterNode.Emit("Left", globals.ChartTimer);
		}
		else if (Input.IsActionJustPressed("Right"))
		{
			_emitterNode.Emit("Right", globals.ChartTimer);
		}
    }
}
