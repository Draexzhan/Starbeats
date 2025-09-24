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

        string[] lines = System.IO.File.ReadAllLines(path);

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (trimmed.StartsWith("--"))
            {
                if (section.Lines.Count > 0)
                {
                    song.Sections.Add(section);
                    section = new RhythmSection();
                }
                continue;
            }

            // BPM line
            if (trimmed.StartsWith("BPM:"))
            {
                song.BPM = float.Parse(trimmed.Substring(4));
                continue;
            }

            section.Lines.Add(new BeatLine(trimmed));
        }

        if (section.Lines.Count > 0)
            song.Sections.Add(section);

        return song;
    }
}

// Player
public partial class RhythmPlayer : Node
{
    private RhythmSong song;
    private float timer = 0f;
    private int currentSection = 0;
    private int currentLine = 0;
    private float lineDuration = 0f;

    public void LoadSong(RhythmSong newSong)
    {
        song = newSong;
        timer = 0f;
        currentSection = 0;
        currentLine = 0;

        // duration of 1 beat
        lineDuration = 60f / song.BPM;
    }

    public override void _Process(double delta)
    {
        if (song == null) return;

        if (currentSection >= song.Sections.Count) return;

        var section = song.Sections[currentSection];
        int linesInSection = section.Lines.Count;

        // duration per line (split beat evenly across lines in section)
        float sectionLineDuration = lineDuration / linesInSection;

        timer += (float)delta;

        if (timer >= sectionLineDuration)
        {
            var line = section.Lines[currentLine];
            GD.Print($"Section {currentSection}, Line {currentLine}: {line.Notes}");

            currentLine++;
            timer -= sectionLineDuration;

            // move to next section if needed
            if (currentLine >= linesInSection)
            {
                currentLine = 0;
                currentSection++;
            }
        }
    }
}




