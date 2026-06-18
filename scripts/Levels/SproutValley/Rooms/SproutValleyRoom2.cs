using Godot;

public partial class SproutValleyRoom2 : Room
{
    public override void OnEnter()
    {
        base.OnEnter();
        GD.Print("SproutValleyRoom2: OnEnter");
    }

    public override void OnExit()
    {
        base.OnExit();
        GD.Print("SproutValleyRoom2: OnExit");
    }
}
