using Godot;
using Library;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace FantasyGame.player.states;

public partial class FallState : PlayerState
{
    [ExportGroup("Transition To States")]
    [Export] private PlayerState _airDashState;
    [Export] private JumpState _jumpState;
    [Export] private IdleState _idleState;
    [Export] private MoveState _moveState;

    public FallState(Player player) : base(player)
    {}

    public override void Enter()
    {
        base.Enter();
    }

    public override IBaseState<Player> ProcessInput(InputEvent @event)
    {
        // Allow base ProcessInput to run for mouselook and attacks
        base.ProcessInput(@event);

        /*
         * If player attempts to jump while falling, check for double jump availability
         * and transition to jump state if available.
         */
        if (Input.IsActionJustPressed("jump") && Parent.CanDoubleJump && !Parent.IsOnFloor())
        {
            return _jumpState;
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
        /*
         * Initialize variables
         */
        Vector2 direction = Vectors.GetCurrentInputDirection();
        
        base.ProcessPhysics(delta);

        /*
         * Use the standard airborne velocity updater to get our velocity, then
         * update our MoveAndSlide.
         */
        UpdateVelocityAirborne(Vectors.GetCurrentInputDirection(), delta);
        Parent.MoveAndSlide();

        /*
         * If player is still airborne at this point, retain the falling state.
         */
        if (!Parent.IsOnFloor()) return null;
        
        /*
         * If player has contacted the ground, emit the landing signal and verify horizontal
         * movement. If moving, return _moveState, if not, return _idleState.
         */
        Parent.EmitSignal(Player.SignalName.Landed);
        if (Parent.Velocity.X != 0 || Parent.Velocity.Z != 0) return _moveState;
        return _idleState;
    }
}