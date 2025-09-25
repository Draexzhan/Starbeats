using Godot;
using System;
using System.Collections.Generic;

// Beat line
public class BeatLine
{
    public string Notes; // e.g., "1010"
    public BeatLine(string notes) => Notes = notes;
}

// Section (measure)
public class RhythmSection
{
    public List<BeatLine> Lines = new List<BeatLine>();
}

// Song
public class RhythmSong
{
    public float BPM;
    public List<RhythmSection> Sections = new List<RhythmSection>();
}

// Parser
public static class RhythmParser
{
    public static RhythmSong ParseFile(string path)
    {
        var song = new RhythmSong();
        var section = new RhythmSection();

        // Open the file using Godot's FileAccess
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"[Parser] Failed to open file at {path}");
            return song;
        }

        GD.Print($"[Parser] Successfully opened file.");

        while (!file.EofReached())
        {
            string line = file.GetLine().Trim();

            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line.StartsWith("--"))
            {
                if (section.Lines.Count > 0)
                {
                    song.Sections.Add(section);
                    section = new RhythmSection();
                }
                continue;
            }

            // BPM line
            if (line.StartsWith("BPM:"))
            {
                if (float.TryParse(line.Substring(4), out float bpm))
                {
                    song.BPM = bpm;
                    GD.Print($"[Parser] BPM set to {song.BPM}");
                }
                else
                {
                    GD.PrintErr($"[Parser] Failed to parse BPM from line: {line}");
                }
                continue;
            }

            section.Lines.Add(new BeatLine(line));
        }

        // Add final section if it has lines
        if (section.Lines.Count > 0)
        {
            song.Sections.Add(section);
        }

        GD.Print($"[Parser] Done. Total sections: {song.Sections.Count}");
        return song;
    }
}


// Player
public partial class RhythmPlayer : Node
{
    [Export] public PackedScene NoteLeftScene { get; set; } //placeholder for actual notes
    [Export] public PackedScene NoteUpScene { get; set; } //placeholder for actual notes
    [Export] public PackedScene NoteRightScene { get; set; } //placeholder for actual notes
    [Export] public PackedScene NoteDownScene { get; set; } //placeholder for actual notes


    private RhythmSong song;
    public float chartTimer = -10f;
    private int currentSection = 0;
    private int currentLine = 0;
    private float lineDuration = 0f;

    public void LoadSong(RhythmSong newSong)
    {
        song = newSong;
        chartTimer = -10f;
        currentSection = 0;
        currentLine = 0;

        // duration of 1 beat
        lineDuration = 60f / song.BPM * 4;

        GD.Print($"[Player] Song loaded. BPM={song.BPM}, Sections={song.Sections.Count}, LineDuration={lineDuration}s per beat");
    }

    private float startOffset = 5f; // trigger lines 5s early
    public string currentChartTrack = "res://songs/team 2 demo v2 (layer 1).mp3";
    private bool songPlaying = false;
    public override void _Process(double delta)
    {
        if (chartTimer >= 0f && !songPlaying)
        {
            songPlaying = true;

            // Create and add the AudioManager node
            var audioManager = new AudioManager();
            AddChild(audioManager);

            // Play the audio file
            audioManager.PlayAudio(currentChartTrack);
        }

        if (song == null) return;

        if (currentSection >= song.Sections.Count) return;

        var section = song.Sections[currentSection];
        int linesInSection = section.Lines.Count;
        float sectionLineDuration = lineDuration / linesInSection;

        chartTimer += (float)delta;

        // Subtract the startOffset initially
        if (startOffset > 0f)
        {
            chartTimer += startOffset;
            startOffset = 0f; // only apply once
        }

        while (chartTimer >= sectionLineDuration && currentSection < song.Sections.Count)
        {
            var line = section.Lines[currentLine];
            GD.Print($"[Player] Section {currentSection}, Line {currentLine}: {line.Notes} (5s early)");

            currentLine++;
            chartTimer -= sectionLineDuration;

            if (currentLine >= linesInSection)
            {
                currentLine = 0;
                currentSection++;

                if (currentSection < song.Sections.Count)
                    section = song.Sections[currentSection];
            }
        }
    }
    private void SpawnNotes(string notes)
    {
        // notes is something like "1010" (left, up, right, down)
        for (int i = 0; i < notes.Length; i++)
        {
            if (notes[0] == '1')
            {
                var noteInstance = (Node2D)NoteLeftScene.Instantiate();

                AddChild(noteInstance);
            }
            if (notes[1] == '1')
            {
                var noteInstance = (Node2D)NoteUpScene.Instantiate();

                AddChild(noteInstance);
            }
            if (notes[2] == '1')
            {
                var noteInstance = (Node2D)NoteRightScene.Instantiate();

                AddChild(noteInstance);
            }
            if (notes[3] == '1')
            {
                var noteInstance = (Node2D)NoteDownScene.Instantiate();

                AddChild(noteInstance);
            }
        }
    }
}

public partial class AudioManager : Node
{
    private AudioStreamPlayer _player;

    public override void _Ready()
    {
        // Create AudioStreamPlayer if it doesn't exist
        _player = new AudioStreamPlayer();
        AddChild(_player);
    }

    /// <summary>
    /// Plays an audio file from the given path (res:// or user://)
    /// </summary>
    /// <param name="path">Path to the audio file (e.g., "res://audio/song.ogg")</param>
    public void PlayAudio(string path)
    {
        var stream = GD.Load<AudioStream>(path);

        if (stream == null)
        {
            GD.PrintErr($"Audio file not found at path: {path}");
            return;
        }

        _player.Stream = stream;
        _player.Play();
        GD.Print($"Playing audio: {path}");
    }

    /// <summary>
    /// Stops the currently playing audio
    /// </summary>
    public void StopAudio()
    {
        if (_player.Playing)
            _player.Stop();
    }
}






