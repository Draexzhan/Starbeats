using Godot;

public partial class Globals : Node
{
    public float ChartTimer { get; set; } = -10f;
    public Star ActiveStar { get; set; } = null;

    public float audioTimer { get; set; } = 0f;
}
