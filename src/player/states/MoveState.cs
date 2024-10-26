using Godot;
using Library;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace FantasyGame.player.states;

/*
 * MoveState
 * This state handles primarily grounded movement in any direction.
 */
public partial class MoveState : PlayerState
{
    /*
     * Possible states to transition to. Assigned from the inspector.
     */
    [ExportGroup("Transition To States")]
    [Export] private FallState _fallState;
    [Export] private IdleState _idleState;
    [Export] private DashState _dashState;
    [Export] private JumpState _jumpState;
    
    /*
     * Constructor
     */
    public MoveState(Player player) : base(player)
    {}
    
    /*
     * Enter()
     * This method runs the base Enter() and resets our double jump ability if it
     * was unavailable.
     */
    public override void Enter()
    { 
        base.Enter();
        if (!Parent.CanDoubleJump) Parent.CanDoubleJump = true;
    }

    public override IBaseState<Player> ProcessInput(InputEvent @event)
    {
        /*
         * Allow base ProcessInput to run for mouse look and attacks
         */
        base.ProcessInput(@event);

        if (Input.IsActionJustPressed("jump")) 
            return _jumpState;
        
        if (Input.IsActionJustPressed("dash") && Parent.DashCooldownTimer.IsStopped()) 
            return _dashState;
        
        return null;
    }

    public override IBaseState<Player> ProcessPhysics(double delta)
    {
        /*
         * Initialize variables
         */
        Vector2 inputDir = Vectors.GetCurrentInputDirection();
        var grounded = Parent.IsOnFloor();
        
        base.ProcessPhysics(delta);

        /*
         * Use the standard UpdateVelocityGrounded() to get our velocity,
         * then update our MoveAndSlide().
         */
        UpdateVelocityGrounded(inputDir, delta);
        Parent.MoveAndSlide();
        
        /*
         * If the player's vertical velocity becomes less than zero, and
         * they are no longer grounded, return _fallState.
         */
        if (Parent.Velocity.Y <= 0 && !grounded) return _fallState;
        
        /*
         * Lastly, verify player is still moving. If not, return _idleState,
         * otherwise retain in MoveState.
         */
        return Parent.Velocity is { X: 0f, Z: 0f } ? _idleState : null;
    }
}