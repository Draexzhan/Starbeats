using Godot;
using System;

public partial class Main : Node
{
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
		switch (starnumber)
		{
			case 0:
				StartChart("res://songs/test.txt");
				break;
			case 1:
				StartChart("res://songs/test.txt");
				break;
			case 2:
				StartChart("res://songs/test.txt");
				break;
			case 3:
				StartChart("res://songs/test.txt");
				break;
			default:
				break;
		}

		var audioManager = GetNode<AudioManager>("CharterPlayer/AudioManager");
		audioManager.InitializeStar(starnumber);
	}

	public void StartChart(string chartPath)
	{
		GD.Print($"[Main] Starting chart: {chartPath}");

		// Parse chart
		var song = RhythmParser.ParseFile(chartPath);

		// Load song into player
		var chartPlayer = GetNode<CharterPlayer>("CharterPlayer");
		chartPlayer.LoadSong(song);
	}
}
