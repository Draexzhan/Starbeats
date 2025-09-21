using Godot;
using System;

public partial class Star : Area2D
{
	[Export] CollisionShape2D StarCollider;
	
	public override void _Ready() 
	{
	}
}
