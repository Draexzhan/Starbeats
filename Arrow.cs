using Godot;
using System;

public partial class Arrow : Node2D
{
    public override void _Ready()
    {
        AnimationPlayer player = GetNode<AnimationPlayer>("AnimationPlayer");
        player.Play("Play");
    }

    public void SetDirection(string direction)
    {
        if (direction == "Up")
        {
            RotationDegrees = 0;
        }
        else if (direction == "Right")
        {
            RotationDegrees = 90;
        }
        else if (direction == "Down")
        {
            RotationDegrees = 180;
        }
        else if (direction == "Left")
        {
            RotationDegrees = 270;
        }
    }

    private void OnAnimationPlayerAnimationFinished(StringName animName)
    {
        GD.Print($"Animation '{animName}' finished!");
        QueueFree();
    }
}
