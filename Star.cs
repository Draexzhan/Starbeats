using Godot;
using System;
using System.Threading;

public partial class Star : Area2D
{
	[Export] public TextureButton StarButton { get; set; }

	[Signal] public delegate void StarClickedEventHandler();

	public override void _Ready()
	{
		if (StarButton != null)
		{
			StarButton.Connect("pressed", new Callable(this, nameof(OnButtonPressed)));
		}
	}

	private void OnButtonPressed()
	{
		GD.Print("Star clicked via TextureButton!"); // debug
		EmitSignal(nameof(StarClicked));             // ping Main
	}

	public void PlaySong()
	{
		//ThreadStart playing audio track
		//start timer
		//read txt file on each 4ths (Create method using Bpm to determine interval between note placements) 
		//how will txt files be read?
		//BPM = 140
		// -- will signal new measure (insert logic to divide amount of lines in meas)
		// 0010
		// 1010
		
	}

}
