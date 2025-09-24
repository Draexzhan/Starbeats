using Godot;
using System;

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
}

