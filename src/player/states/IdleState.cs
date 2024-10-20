using Godot;

public partial class IdleState : PlayerState
{
    public IdleState(Player player) : base(player)
    {
        // Additional constructor processing goes here.
    }

    public override IBaseState<Player> ProcessInput(InputEvent @event)
    {
        return null;
    }

    public override IBaseState<Player> ProcessFrame(double delta)
    {
        return null;
    }

    public override IBaseState<Player> ProcessPhysics(double delta)
    {
        
        return null;
    }
}