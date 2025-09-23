using Godot;
using System;

public partial class StarTest : Area2D
{
	[Signal] public delegate void StarClickedEventHandler();

	public override void _Ready()
	{
		// Make sure the CollisionShape2D exists and input is pickable
		Connect("input_event", new Callable(this, nameof(OnInputEvent)));
	}

	private void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				GD.Print("Star clicked!");        // debug
				EmitSignal(nameof(StarClicked));  // “ping” Main
			}
		}
	}
}
