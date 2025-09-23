using Godot;
using System;

public partial class Star : Area2D
{
	[Export] public CollisionShape2D StarCollider {get;set;}
	
	public override void _Ready() 
	{
	}
}
