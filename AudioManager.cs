using Godot;
using System.Threading.Tasks;

public partial class AudioManager : Node
{
    private AudioStreamPlayer masterPlayer; //always playing in bg. muted
    public float audioTimer = 0f;
    private Globals globals;
    private AudioStreamPlayer Layer0;
    private AudioStreamPlayer Layer1;
    private AudioStreamPlayer Layer2;
    private AudioStreamPlayer Layer3;


    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");

        // we create the first track and loop it repeatedly to keep everything in time via audioTimer. this audio is always muted
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


	public void InitializeStar(int starnumber)
	{
		AudioStreamPlayer song = starnumber switch
        {
            0 => GetNode<AudioStreamPlayer>("Layer0"),
            1 => GetNode<AudioStreamPlayer>("Layer1"),
            2 => GetNode<AudioStreamPlayer>("Layer2"),
            3 => GetNode<AudioStreamPlayer>("Layer3"),
            _ => GetNode<AudioStreamPlayer>("error, invalid star number!"),
        };

		FadeIn(song, 3f);
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


