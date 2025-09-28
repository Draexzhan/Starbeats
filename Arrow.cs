using Godot;
using System;

public partial class Arrow : Node2D
{

    public static float PREP_TIME_SECONDS = 5f;
    public float hitTime = 0f;
    public string direction;

    public override void _Ready()
    {
        AnimationPlayer player = GetNode<AnimationPlayer>("AnimationPlayer");
        player.Play("Play");

    }

    public void setHitTimer(float audioTimer) {
        hitTime = audioTimer + PREP_TIME_SECONDS;
    }

    public void SetDirection(string dir)
    {
        direction = dir;
        RotationDegrees = direction switch
        {
            "Up" => 0,
            "Right" => 90,
            "Down" => 180,
            "Left" => 270,
            _ => 0,
        };
    }

    private void OnAnimationPlayerAnimationFinished(StringName animName)
    {
        QueueFree();
    }
}
