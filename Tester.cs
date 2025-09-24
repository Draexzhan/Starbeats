using Godot;
using System;

public partial class RhythmTester : Node
{
    private RhythmPlayer player;

    public override void _Ready()
    {
        GD.Print("Testing RhythmPlayer...");

        // Load the song
        var song = RhythmParser.ParseFile("res://songs/test.txt");

        // Print BPM and sections for debugging
        GD.Print($"BPM: {song.BPM}");
        GD.Print($"Sections: {song.Sections.Count}");

        // Create and add RhythmPlayer
        player = new RhythmPlayer();
        AddChild(player);

        // Load song into player
        player.LoadSong(song);
    }
}


