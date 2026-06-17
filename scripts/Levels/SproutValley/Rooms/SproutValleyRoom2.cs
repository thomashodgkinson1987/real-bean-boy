using Godot;

public partial class SproutValleyRoom2 : Room
{
    public override void OnEnter()
    {
        GD.Print("SproutValleyRoom2: OnEnter");
    }

    public override void OnExit()
    {
        GD.Print("SproutValleyRoom2: OnExit");
    }
}
