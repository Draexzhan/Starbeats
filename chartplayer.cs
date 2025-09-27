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
public partial class ChartPlayer : Node
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
    public override void _Process(double delta)
    {
        GD.Print(globals.audioTimer);
        if (song == null) return;

        if (currentSection >= song.Sections.Count) return;

        var section = song.Sections[currentSection];
        int linesInSection = section.Lines.Count;
        float sectionLineDuration = lineDuration / linesInSection;

        globals.ChartTimer += (float)delta;
        globals.audioTimer += (float)delta;

        while (globals.audioTimer + noteDelay >= sectionLineDuration && currentSection < song.Sections.Count)
        {
            float tempTimer = globals.audioTimer + noteDelay;
            var line = section.Lines[currentLine];
            GD.Print($"[Player] Section {currentSection}, Line {currentLine}: {line.Notes} (offset {noteDelay}s)");

            globals.ActiveStar.SpawnNote("Left"); // spawn the note earlier

            currentLine++;
            globals.audioTimer -= sectionLineDuration;

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
                globals.ActiveStar.SpawnNote("Left");
            }
            if (notes[1] == '1')
            {
                globals.ActiveStar.SpawnNote("Up");
            }
            if (notes[2] == '1')
            {
                globals.ActiveStar.SpawnNote("Right");
            }
            if (notes[3] == '1')
            {
                globals.ActiveStar.SpawnNote("Down");
            }
        }
    }
}

public partial class AudioManager : Node
{
    private AudioStreamPlayer masterPlayer; //always playing in bg. muted
    public float audioTimer = 0f;
    private Globals globals;
    private AudioStreamPlayer2D Layer1;
    private AudioStreamPlayer2D Layer2;
    private AudioStreamPlayer2D Layer3;
    private AudioStreamPlayer2D Layer4;


    public override void _Ready()
    {
        Layer1 = GetNode<AudioStreamPlayer2D>("CollectMyAudios/Layer1");
        Layer2 = GetNode<AudioStreamPlayer2D>("CollectMyAudios/Layer2");
        Layer3 = GetNode<AudioStreamPlayer2D>("CollectMyAudios/Layer3");
        Layer4 = GetNode<AudioStreamPlayer2D>("CollectMyAudios/Layer4");
        //we create the first track and loop it repeatedly to keep everything in time via audioTimer. this audio is always muted
        masterPlayer = new AudioStreamPlayer();
        AddChild(masterPlayer);


    }

    public override void _Process(double delta)
    {
        //looping method
        if (masterPlayer.Playing && masterPlayer.GetPlaybackPosition() >= masterPlayer.Stream.GetLength())
        {
            masterPlayer.Stop();
            masterPlayer.Play(); // restart
        }
        globals.audioTimer = GetAudioTime();
    }

    /// Plays an audio file from the given path (res:// or user://)
    /// <param name="path">Path to the audio file (e.g., "res://audio/song.ogg")</param>
    public void PlayAudio(string path)
    {
        var stream = GD.Load<AudioStream>(path);

        if (stream == null)
        {
            GD.PrintErr($"Audio file not found at path: {path}");
            return;
        }

        masterPlayer.Stream = stream;
        masterPlayer.Play();
        GD.Print($"Playing audio: {path}");
    }

    /// Stops the currently playing audio
    public void StopAudio()
    {
        if (masterPlayer.Playing)
            masterPlayer.Stop();
    }

    //this grabs the current audio timing of our track
    public float GetAudioTime()
    {
        if (masterPlayer == null)
        {
            GD.PrintErr("[AudioManager] Tried to get audio time but no player exists!");
            return 0f;
        }
        return (float)masterPlayer.GetPlaybackPosition();
    }
    //call this when rhythm game starts
    public async void FadeIn(AudioStreamPlayer layer, float duration)
    {
        layer.VolumeDb = -80f; // Start silent
        layer.Play();

        float startTime = 0f;
        float targetVolume = 0f; // 0 dB = full volume
        float startVolume = layer.VolumeDb;

        while (startTime < duration)
        {
            await Task.Delay(16); // roughly one frame at 60fps
            startTime += 0.016f;

            float t = startTime / duration;
            layer.VolumeDb = Mathf.Lerp(startVolume, targetVolume, t);
        }

        layer.VolumeDb = targetVolume; // ensure exact target at end
    }

    //call this when rhythm game is failed
    public async void FadeOut(AudioStreamPlayer layer, float duration)
    {
        layer.VolumeDb = 0f; // Start normal
        layer.Play();

        float startTime = 0f;
        float targetVolume = -80f; // 0 dB = full volume
        float startVolume = layer.VolumeDb;

        while (startTime < duration)
        {
            await Task.Delay(16); // roughly one frame at 60fps
            startTime -= 0.016f;

            float t = startTime / duration;
            layer.VolumeDb = Mathf.Lerp(startVolume, targetVolume, t);
        }

        layer.VolumeDb = targetVolume; // ensure exact target at end
    }
}







