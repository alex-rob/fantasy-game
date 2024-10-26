using System.Diagnostics;
using Godot;
using Library;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace FantasyGame.player.states;

public partial class IdleState : PlayerState
{
    // Potential states
    [Export] private FallState _fallState;
    [Export] private JumpState _jumpState;
    [Export] private PlayerState _moveState;
    [Export] private PlayerState _dashState;


    public IdleState(Player player) : base(player)
    {
        // Additional constructor processing can go here.
    }

    public override void Enter()
    {
        base.Enter();
        // Reset our double jump
        if (!Parent.CanDoubleJump) Parent.CanDoubleJump = true;
    }

    public override IBaseState<Player> ProcessInput(InputEvent @event)
    {
        // Allow the base ProcessInput to run first for mouselook behavior and attacks
        base.ProcessInput(@event);

        if (Input.IsActionJustPressed("jump") && Parent.IsOnFloor())
        {
            return _jumpState;
        }

        if (Input.IsActionJustPressed("dash") && Parent.DashCooldownTimer.IsStopped())
        {
            return _dashState;
        }

        if (Vectors.GetCurrentInputDirection() != Vector2.Zero && Parent.IsOnFloor())
        {
            return _moveState;
        }

        return null;
    }

    /*
     * UpdateVelocityGrounded()
     * This method is overridden to lock horizontal momentum while idle.
     */
    protected override Vector3 UpdateVelocityGrounded(Vector2 inputDir, double delta)
    {
        var grounded = Parent.IsOnFloor();
        Debug.Assert(grounded, "Attempting to updating grounded velocity while airborne");
        
        Vector3 targetVelocity = Parent.Velocity;
        
        /*
         * Apply gravity to the vertical velocity
         */
        targetVelocity += Parent.GetGravity() * (float)delta;
        
        /*
         * Lock the player's horizontal movement
         */
        targetVelocity.X = targetVelocity.Z = 0;
        
        return targetVelocity;
    }

    public override IBaseState<Player> ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        /*
         * Run the UpdateVelocityGrounded method, passing a zero vector for inputDir.
         */
        UpdateVelocityGrounded(Vector2.Zero, delta);
        
        Parent.MoveAndSlide();

        /*
         * If something has happened that is causing the player's velocity to
         * be < 0, transition to _fallState.
         */
        if (!Parent.IsOnFloor() && Parent.Velocity.Y < 0)
        {
            return _fallState;
        }

        return null;
    }
}