using Godot;
using Library;

public partial class IdleState : PlayerState
{
    // Potential states
    [Export] PlayerState fall_state;
    [Export] PlayerState jump_state;
    [Export] PlayerState move_state;
    [Export] PlayerState dash_state;


    public IdleState(Player player) : base(player)
    {
        // Additional constructor processing can go here.
    }

    public override IBaseState<Player> ProcessInput(InputEvent @event)
    {
        // Allow the base ProcessInput to run first for mouselook behavior and attacks
        base.ProcessInput(@event);

        if (Input.IsActionJustPressed("jump") && _parent.IsOnFloor())
        {
            return jump_state;
        }

        if (Input.IsActionJustPressed("dash") && _parent.dashCooldownTimer.IsStopped())
        {
            return dash_state;
        }

        if (Vectors.GetCurrentInputDirection() != Vector2.Zero && _parent.IsOnFloor())
        {
            return move_state;
        }

        return null;
    }

    public override IBaseState<Player> ProcessPhysics(double delta)
    {
        Vector3 temp = _parent.Velocity;
        temp += _parent.GetGravity() * (float)delta;
        _parent.Velocity = temp;
        _parent.MoveAndSlide();

        if (!_parent.IsOnFloor())
        {
            return fall_state;
        }

        return null;
    }
}