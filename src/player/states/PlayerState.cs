/*
    PlayerState class

    The PlayerState class is a base class for state machine states which will all inherit this class.
    Basic functionality and default methods are defined for player states here. This class inherits
    the IBaseState interface to define basic functionality.
*/

#nullable enable
using Godot;
using System;

public partial class PlayerState : IBaseState<Player>
{
    [Export] public string? animationName;
    [Export] public float moveSpeed;
    protected Player _parent;

    /*
        Constructor
        As a requirement, the _parent field must be supplied with the player in the constructor
        for any PlayerState class.
    */
    public PlayerState(Player player)
    {
        _parent = player;
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
			_parent.RotateY(-motionEvent.Relative.X * _parent.lookSensitivity * ((float)Math.PI/180));
			_parent.viewportPivot.RotateX(-motionEvent.Relative.Y * _parent.lookSensitivity * ((float)Math.PI/180));

            // Lock the vertical rotation of the viewport to +/- 90 degrees
			if (_parent.viewportPivot.Rotation.X > Math.PI/2 || _parent.viewportPivot.Rotation.X < -Math.PI/2) {
				_parent.viewportPivot.Rotation = new Vector3((float)Mathf.Clamp(_parent.viewportPivot.Rotation.X, -Math.PI/2, Math.PI/2), _parent.viewportPivot.Rotation.Y, _parent.viewportPivot.Rotation.Z);
			}
		}

        if (Input.IsActionJustPressed("attack") && _parent.canAttack)
		{
			_parent.FireBasicAttack();
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
}