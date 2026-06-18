using Godot;

public partial class SproutValleyRoom3 : Room
{
    public override void OnEnter()
    {
        base.OnEnter();
        GD.Print("SproutValleyRoom3: OnEnter");
    }

    public override void OnExit()
    {
        base.OnExit();
        GD.Print("SproutValleyRoom3: OnExit");
    }
}
