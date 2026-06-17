using Godot;

public partial class SproutValleyRoom1 : Room
{
    public override void OnEnter()
    {
        GD.Print("SproutValleyRoom1: OnEnter");
    }

    public override void OnExit()
    {
        GD.Print("SproutValleyRoom1: OnExit");
    }
}
