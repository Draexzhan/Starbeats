using Godot;

public partial class HitKeyEmitter : Node
{
    [Signal]
    public delegate void HitKeyEventHandler(string message);

    public float timer = -10f;

    public void SetTimer(float t)
    {
        timer = t;
    }

    public void Emit(string key, float timing)
    {
        EmitSignal(SignalName.HitKey, key, timing);
    }
}
