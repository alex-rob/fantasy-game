using Godot;
using Library;

public partial class IdleState : PlayerState
{
    // Potential states
    [Export] PlayerState fallState;
    [Export] PlayerState jumpState;
    [Export] PlayerState moveState;
    [Export] PlayerState dashState;


    public IdleState(Player player) : base(player)
    {
        // Additional constructor processing can go here.
    }

    public override void Enter()
    {
        base.Enter();
        // Reset our double jump
        if (!_parent.canDoubleJump) _parent.canDoubleJump = true;
    }

    public override IBaseState<Player> ProcessInput(InputEvent @event)
    {
        // Allow the base ProcessInput to run first for mouselook behavior and attacks
        base.ProcessInput(@event);

        if (Input.IsActionJustPressed("jump") && _parent.IsOnFloor())
        {
            return jumpState;
        }

        if (Input.IsActionJustPressed("dash") && _parent.dashCooldownTimer.IsStopped())
        {
            return dashState;
        }

        if (Vectors.GetCurrentInputDirection() != Vector2.Zero && _parent.IsOnFloor())
        {
            return moveState;
        }

        return null;
    }

    public override IBaseState<Player> ProcessPhysics(double delta)
    {
        // Add gravity
        base.ProcessPhysics(delta);
        _parent.MoveAndSlide();

        if (!_parent.IsOnFloor() && _parent.Velocity.Y < 0)
        {
            return fallState;
        }

        return null;
    }
}