using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
public partial class CharterPlayer : Node
{

    private RhythmSong song;
    private Globals globals;
    private int currentSection = 0;
    private int currentLine = 0;
    private float lineDuration = 0f;

    private float noteDelay = -5f;

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");

    }


    public void LoadSong(RhythmSong newSong)
    {
        song = newSong;

        // duration of 1 full beat (in seconds)
        lineDuration = 60f / song.BPM * 4;

        float accumulated = 0f;
        currentSection = 0;
        currentLine = 0;

        for (int s = 0; s < song.Sections.Count; s++)
        {
            var section = song.Sections[s];
            float sectionLineDuration = lineDuration / section.Lines.Count;
            float linesDuration = section.Lines.Count * sectionLineDuration;

            if (accumulated + linesDuration > globals.audioTimer)
            {
                currentSection = s;
                int lineIndex = (int)((globals.audioTimer - accumulated) / sectionLineDuration);
                currentLine = Mathf.Clamp(lineIndex, 0, section.Lines.Count - 1);
                break;
            }

            accumulated += linesDuration;
        }

        GD.Print($"[Player] Song loaded. BPM={song.BPM}, Sections={song.Sections.Count}, LineDuration={lineDuration}s per beat");
        GD.Print($"[Player] Starting at Section {currentSection}, Line {currentLine}, Time {globals.audioTimer}s");
    }


    private float startOffset = 5f; // trigger lines 5s early
    private float nextLineTime = 0f;

    public override void _Process(double delta)
    {
        if (song == null) return;
        if (currentSection >= song.Sections.Count) return;

        globals.audioTimer += (float)delta;

        var section = song.Sections[currentSection];
        float sectionLineDuration = lineDuration / section.Lines.Count;

        // Trigger next line only when it's time
        if (globals.audioTimer + noteDelay >= nextLineTime)
        {
            var line = section.Lines[currentLine];
            GD.Print($"[Player] Section {currentSection}, Line {currentLine}: {line.Notes}");

            SpawnNotes(line.Notes);

            currentLine++;
            GD.Print(nextLineTime);
            nextLineTime += sectionLineDuration; // schedule next line

            // Move to next section
            if (currentLine >= section.Lines.Count)
            {
                currentLine = 0;
                currentSection++;
                nextLineTime = globals.audioTimer + noteDelay; // reset for next section
            }
        }
    }

    private void SpawnNotes(string notes)
    {
        GD.Print("notes?", notes);
        if (notes[0] == '1') globals.ActiveStar.SpawnNote("Left", globals.audioTimer);
        if (notes[1] == '1') globals.ActiveStar.SpawnNote("Up", globals.audioTimer);
        if (notes[2] == '1') globals.ActiveStar.SpawnNote("Right", globals.audioTimer);
        if (notes[3] == '1') globals.ActiveStar.SpawnNote("Down", globals.audioTimer);
    }
}
