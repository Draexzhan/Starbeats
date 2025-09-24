using Godot;
using System;

public partial class Main : Node
{
	[Export] public PackedScene Star { get; set; }

	private Camera2D _camera;

	// Camera drag state
	private bool _dragging = false;
	private Vector2 _lastMousePos = Vector2.Zero;

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

		// Grab reference to Camera2D (assuming it's a child of Main)
		_camera = GetNode<Camera2D>("Camera2D");

		// Enable input processing for this node
		SetProcessInput(true);

		 // Spawn a star
        Star starInstance = Star.Instantiate<Star>();
        AddChild(starInstance); // important: must add to scene first

        // Connect the StarClicked signal to Main
        starInstance.Connect("StarClicked", new Callable(this, nameof(OnStarClicked)));
	}

	private void OnStarClicked()
	{
		//when star is pressed, trigger rhythm sequence
		GD.Print("Main received a star click!");
		currentState = (int)GameState.Rhythm;
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
			Vector2 delta = motionEvent.Position - _lastMousePos;
			_camera.GlobalPosition -= delta; // pan opposite to drag
			_lastMousePos = motionEvent.Position;
		}

		// Other input logic
		if (@event is InputEventMouseButton clickEvent && clickEvent.Pressed)
		{
			GD.Print("Click!");
			currentState = (int)GameState.Rhythm;
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
}
