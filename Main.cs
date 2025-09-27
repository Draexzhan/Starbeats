using Godot;
using System;

public partial class Main : Node
{
	[Export] public PackedScene Star { get; set; }

	private Camera2D _camera;

	// Camera drag state
	private bool _dragging = false;
	private Vector2 _lastMousePos = Vector2.Zero;
	private Globals globals;

	// Game state
	public enum GameState
	{
		StarSelect = 0,
		Rhythm = 1
	};
	public int currentState;

	public override void _Ready()
	{
		currentState = (int)GameState.StarSelect;
		globals = GetNode<Globals>("/root/Globals");
		globals.ActiveStar = GetNode<Star>("Constellation/LeftStar");
		GD.Print(globals.ActiveStar.ToString());

		// Grab reference to Camera2D (assuming it's a child of Main)
		_camera = GetNode<Camera2D>("Camera2D");
		
		//placeholder for getting and playing chart
		StartChart("res://songs/test.txt");


	}

	private void OnHitKey()
	{

		//when star is pressed, trigger rhythm sequence
		GD.Print("Main received a star click!");
		currentState = (int)GameState.Rhythm;
		//Star.InitializeStar() placeholder
		InitializeStar(1);
	}

	public override void _Input(InputEvent @event)
	{
		// Camera dragging logic
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (mouseEvent.Pressed)
				{
					_dragging = true;
					_lastMousePos = mouseEvent.Position;
				}
				else
				{
					_dragging = false;
				}
			}
		}
		else if (@event is InputEventMouseMotion motionEvent && _dragging)
		{
			_camera.GlobalPosition -= motionEvent.Relative;
		}

		// Other input logic
		if (@event is InputEventMouseButton clickEvent && clickEvent.Pressed)
		{
			GD.Print("Click!");
			currentState = (int)GameState.Rhythm;
			InitializeStar(1);
		}

		// Rhythm game inputs
		if (currentState == (int)GameState.Rhythm)
		{
			if (Input.IsActionJustPressed("Up"))
				GD.Print("Up!");
			if (Input.IsActionJustPressed("Down"))
				GD.Print("Down!");
			if (Input.IsActionJustPressed("Left"))
				GD.Print("Left!");
			if (Input.IsActionJustPressed("Right"))
				GD.Print("Right!");
		}
	}
	public void InitializeStar(int starnumber)
	{
		if (starnumber == 0)
		{
			StartChart("res://songs/test.txt");
			var song = GetNode<AudioStreamPlayer>("ChartPlayer/CollectMyAudios/Layer0");
			GetNode<AudioManager>("ChartPlayer").FadeIn(song, 3f);
		}
		else if (starnumber == 1)
		{
			StartChart("res://songs/test.txt");
			var song = GetNode<AudioStreamPlayer>("ChartPlayer/CollectMyAudios/Layer1");
			GetNode<AudioManager>("ChartPlayer").FadeIn(song, 3f);
		}
		else if (starnumber == 2)
		{
			StartChart("res://songs/test.txt");
			var song = GetNode<AudioStreamPlayer>("ChartPlayer/CollectMyAudios/Layer2");
			GetNode<AudioManager>("ChartPlayer").FadeIn(song, 3f);
		}
		else if (starnumber == 3)
		{
			StartChart("res://songs/test.txt");
			var song = GetNode<AudioStreamPlayer>("ChartPlayer/CollectMyAudios/Layer3");
			GetNode<AudioManager>("ChartPlayer").FadeIn(song, 3f);
		}
	}
	public void StartChart(string chartPath)
	{
		GD.Print($"[Main] Starting chart: {chartPath}");

		// Parse chart
		var song = RhythmParser.ParseFile(chartPath);

		// Load song into player
		GetNode<ChartPlayer>("ChartPlayer").LoadSong(song);
	}
}
