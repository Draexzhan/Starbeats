using Godot;
using System;
using System.Threading;


public partial class Star : Area2D
{
	public int LEADUP_SECONDS = 5;

	[Export] public string inputKey;
	private HitKeyEmitter _emitterNode;
	private RhythmPlayer _chartPlayer;

	PackedScene arrow = GD.Load<PackedScene>("res://arrow.tscn");

	public override void _Ready()
	{
		if (inputKey == null)
		{
			GD.PrintErr("no event action");
		}
		_emitterNode = GetNode<HitKeyEmitter>("/root/Main/HitKeyEmitter");
		// _chartPlayer = GetNode<RhythmPlayer>("/root/Main/ChartPlayer");
	}

	public void SpawnNote()
	{
		Arrow a = arrow.Instantiate<Arrow>();
		AddChild(a);
		a.SetDirection(inputKey);
	}

    public override void _Process(double delta)
    {
		if (Input.IsActionPressed(inputKey))
		{
			_emitterNode.Emit(inputKey, 4);
			SpawnNote();
		}
    }
}
