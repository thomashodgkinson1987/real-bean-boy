using Godot;

public partial class SproutValleyLevel : Level
{
    public override void OnEnter()
    {
        GD.Print("SproutValleyLevel: OnEnter");
    }

    public override void OnExit()
    {
        GD.Print("SproutValleyLevel: OnExit");
    }
}