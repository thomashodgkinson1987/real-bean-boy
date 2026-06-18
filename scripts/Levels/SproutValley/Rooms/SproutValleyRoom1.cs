using Godot;

public partial class SproutValleyRoom1 : Room
{
    public override void OnEnter()
    {
        base.OnEnter();
        GD.Print("SproutValleyRoom1: OnEnter");
    }

    public override void OnExit()
    {
        base.OnExit();
        GD.Print("SproutValleyRoom1: OnExit");
    }
}
