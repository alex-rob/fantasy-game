using Godot;
using Library;

public partial class JumpState : PlayerState
{
    [Export] PlayerState airDashState;
    [Export] PlayerState fallState;
    [Export] PlayerState idleState;
    [Export] PlayerState moveState;

    [Export] float jumpVelocity = 7f;

    public JumpState(Player player) : base(player)
    {}

    public void Jump()
    {
        Vector3 velocity = _parent.Velocity;
        velocity.Y = jumpVelocity;
        _parent.Velocity = velocity;
    }

    public override void Enter()
    {
        base.Enter();

        if (_parent.IsOnFloor())
        {
            Jump();
            _parent.EmitSignal(Player.SignalName.leftGround);
        }
    }

    public override IBaseState<Player> ProcessInput(InputEvent @event)
    {
        // Allow base ProcessInput to run for mouselook and attacks
        base.ProcessInput(@event);

        if (Input.IsActionJustPressed("jump") && _parent.canDoubleJump && !_parent.IsOnFloor())
        {
            Jump();
            _parent.canDoubleJump = false;
        }  

        if (Input.IsActionJustPressed("dash") && _parent.dashCooldownTimer.IsStopped() && !_parent.IsOnFloor())
        {
            return airDashState;
        }

        return null;
    }

    public override IBaseState<Player> ProcessPhysics(double delta)
    {
        // Add gravity
        base.ProcessPhysics(delta);

        //TODO Add other directional movement


        _parent.MoveAndSlide();

        if (_parent.Velocity.Y < 0)
        {
            return fallState;
        }

        if (_parent.IsOnFloor())
        {
            _parent.EmitSignal(Player.SignalName.landed);
            if (_parent.Velocity.X != 0 || _parent.Velocity.Z != 0) return moveState;
            return idleState;
        }

        return null;
    }
}