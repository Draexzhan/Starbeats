using Godot;
using System;

public partial class RhythmTester : Node
{
    private RhythmPlayer player;

    public override void _Ready()
    {
        GD.Print("Starting Rhythm Tester...");
        
        // Parse chart file
        var song = RhythmParser.ParseFile("res://songs/test.txt");

        GD.Print($"Loaded song with BPM={song.BPM}, Sections={song.Sections.Count}");

        // Create RhythmPlayer node
        player = new RhythmPlayer();
        AddChild(player);

        // Load song into player
        player.LoadSong(song);
    }
}


