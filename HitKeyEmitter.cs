using Godot;

public partial class HitKeyEmitter : Node
{
    [Signal]
    public delegate void HitKeyEventHandler(string message);

    public void Emit(string key, float timing)
    {
        EmitSignal(SignalName.HitKey, key, timing);
    }
}
