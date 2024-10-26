using Godot;
using Library;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace FantasyGame.player.states;

public partial class JumpState : PlayerState
{
    /*
     * Possible states to transition to. Assigned from the inspector.
     */
    [ExportGroup("Transition To States")]
    [Export] private PlayerState _airDashState;
    [Export] private PlayerState _fallState;
    [Export] private IdleState _idleState;
    [Export] private PlayerState _moveState;
    
    /*
     * Constructor
     */
    public JumpState(Player player) : base(player)
    {}

    /*
     * Enter()
     * The method will Jump() the player if they are grounded
     */
    public override void Enter()
    {
        /*
         * Initialize variables
         */
        var grounded = Parent.IsOnFloor();
        
        /*
         * Run the base Enter() method to play any associated animations
         * or sounds.
         */
        base.Enter();
        
        /*
         * The player will attempt to Jump(). If the player was grounded, they
         * will also emit the LeftGround signal.
         */
        Jump();
        if (grounded) Parent.EmitSignal(Player.SignalName.LeftGround);
    }

    public override IBaseState<Player> ProcessInput(InputEvent @event)
    {
        // Allow base ProcessInput to run for mouselook and attacks
        base.ProcessInput(@event);

        /*
         * If player attempts to jump while in a jump, run the Enter() method again
         * so that we replay any suitable animations or sounds.
         * If a double jump is available, it will be used.
         */
        if (Input.IsActionJustPressed("jump"))
        {
            Enter();
        }  

        /*
         * If player attempts to dash during a jump, make sure the cooldown is complete and verify
         * that the player is airborne, then enter the air dash state
         */
        if (Input.IsActionJustPressed("dash") && Parent.DashCooldownTimer.IsStopped() && !Parent.IsOnFloor())
        {
            return _airDashState;
        }

        return null;
    }

    public override IBaseState<Player> ProcessPhysics(double delta)
    {
        base.ProcessPhysics(delta);

        /*
         * Use the standard airborne velocity updater to get our velocity, then
         * update our MoveAndSlide.
         */
        UpdateVelocityAirborne(Vectors.GetCurrentInputDirection(), delta);
        Parent.MoveAndSlide();

        /*
         * If the player's velocity is less than zero, the player has reached the apex of their
         * jump and must move to the falling state.
         */
        if (Parent.Velocity.Y < 0)
        {
            return _fallState;
        }

        /*
         * If the player somehow contacts the floor while rising, process as if we
         * are landing normally like in the falling state.
         */
        if (Parent.IsOnFloor())
        {
            Parent.EmitSignal(Player.SignalName.Landed);
            if (Parent.Velocity.X != 0 || Parent.Velocity.Z != 0) return _moveState;
            return _idleState;
        }

        return null;
    }
    
    /*
     * Jump()
     * The function will first verify if the player is attempting to double jump,
     * using it if available. Otherwise, the jump velocity will be added to
     * current velocity and returned.
     */
    private void Jump()
    {
        /*
         * Initialize variables
         */
        Vector3 velocity = Parent.Velocity;
        var airborne = !Parent.IsOnFloor();

        /*
         * If player is airborne, verify double jump is available before jumping.
         */
        if (airborne && !Parent.CanDoubleJump) return;
        
        /*
         * Add the player's jump velocity to their current velocity. If double jumping,
         * disable double jump.
         */
        velocity.Y = Parent.JumpVelocity;
        Parent.Velocity = velocity;
        if (airborne) Parent.CanDoubleJump = false;
    }
}