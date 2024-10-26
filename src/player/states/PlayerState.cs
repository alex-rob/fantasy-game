/*
    PlayerState class

    The PlayerState class is a base class for state machine states which will all inherit this class.
    Basic functionality and default methods are defined for player states here. This class inherits
    the IBaseState interface to define basic functionality.
*/

#nullable enable
using System;
using System.Diagnostics;
using Godot;
using Library;

namespace FantasyGame.player.states;

public partial class PlayerState : IBaseState<Player>
{
    [Export] public string? AnimationName;
    protected readonly Player Parent;

    /*
    Constructor
    As a requirement, the Parent field must be supplied with the player in the constructor
    for any PlayerState class.
*/
    public PlayerState(Player player)
    {
        Parent = player;
    }

    /*
    Enter()
    Behavior that occurs when the player enters the state.
*/
    public virtual void Enter()
    {
        // TODO: implement animation playing and play an animation/sound here.
        return;
    }

    /*
    Exit()
    Behavior that occurs when the player leaves the state.
*/
    public virtual void Exit()
    {
        return;
    }

    /*
    ProcessInput(@event)
    Accepts an InputEvent object and processes any relevant inputs for the state and
    input handling logic.
    Returning type is the class of which state we are transitioning to, where null
    indicates no transition needed.
    The player will always have control over the mouselook, so that behavior is defined here.
*/
    public virtual IBaseState<Player>? ProcessInput(InputEvent @event)
    {
        // Handle Mouselook
        if (@event is InputEventMouseMotion motionEvent) {
            Parent.RotateY(-motionEvent.Relative.X * Parent.LookSensitivity * ((float)Math.PI/180));
            Parent.ViewportPivot.RotateX(-motionEvent.Relative.Y * Parent.LookSensitivity * ((float)Math.PI/180));

            // Lock the vertical rotation of the viewport to +/- 90 degrees
            if (Parent.ViewportPivot.Rotation.X > Math.PI/2 || Parent.ViewportPivot.Rotation.X < -Math.PI/2) {
                Parent.ViewportPivot.Rotation = new Vector3((float)Mathf.Clamp(Parent.ViewportPivot.Rotation.X, -Math.PI/2, Math.PI/2), Parent.ViewportPivot.Rotation.Y, Parent.ViewportPivot.Rotation.Z);
            }
        }

        if (Input.IsActionJustPressed("attack") && Parent.CanAttack)
        {
            Parent.FireBasicAttack();
        }
        
        return null;
    }

    /*
    ProcessFrame(delta)
    Behavior that occurs on every frame. To be used during the player's _Process layer.
    Returning type is the class of which state we are transitioning to, where null
    indicates no transition needed.
*/
    public virtual IBaseState<Player>? ProcessFrame(double delta)
    {
        return null;
    }

    /*
    ProcessPhysics(delta)
    Behavior that occurs on every frame. To be used during the player's _ProcessPhysics layer.
    Returning type is the class of which state we are transitioning to, where null
    indicates no transition needed.
*/
    public virtual IBaseState<Player>? ProcessPhysics(double delta)
    {
        return null;
    }
    
    /*
     * UpdateVelocityAirborne()
     * Default behavior for handling airborne movement. This can be overridden by inheriting
     * state classes for clarity. Asserts that the Parent is airborne, and assumes that we are not dashing.
     */
    protected virtual Vector3 UpdateVelocityAirborne(Vector2 inputDir, double delta)
    {
        var airborne = !Parent.IsOnFloor();
        Debug.Assert(airborne, "Attempting to update airborne velocity while grounded.");
        
        Vector3 targetVelocity = Parent.Velocity;
        Vector3 direction = (Parent.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        
        /*
         * Apply gravity to the vertical velocity
         */
        targetVelocity += Parent.GetGravity() * (float)delta;

        /*
         * Determine whether the player is trying to move in a direction, or stop.
         */
        targetVelocity = direction == Vector3.Zero
            ? Vectors.MoveTowardsXZ(targetVelocity, Vector3.Zero, Parent.AirDeceleration)
            : Vectors.MoveTowardsXZ(targetVelocity, direction * Parent.MaxSpeed, Parent.AirAcceleration); 
        
        return targetVelocity;
    }

    /*
     * UpdateVelocityGrounded()
     * Default behavior for handling grounded movement. This can be overridden by inheriting
     * state classes for clarity.
     */
    protected virtual Vector3 UpdateVelocityGrounded(Vector2 inputDir, double delta)
    {
        var grounded = Parent.IsOnFloor();
        Debug.Assert(grounded, "Attempting to updating grounded velocity while airborne");
        
        Vector3 targetVelocity = Parent.Velocity;
        Vector3 direction = (Parent.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        
        /*
         * Apply gravity to the vertical velocity
         */
        targetVelocity += Parent.GetGravity() * (float)delta;
        
        /*
         * Determine whether the player is trying to move in a direction, or stop.
         */
        targetVelocity = direction == Vector3.Zero
            ? Vectors.MoveTowardsXZ(targetVelocity, Vector3.Zero, Parent.Deceleration)
            : Vectors.MoveTowardsXZ(targetVelocity, direction * Parent.MaxSpeed, Parent.Acceleration);
        
        return targetVelocity;
    }
    
    
}