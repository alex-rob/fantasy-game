/*
    PlayerState class

    The PlayerState class is a base class for state machine states which will all inherit this class.
    Basic functionality and default methods are defined for player states here. This class inherits
    the IBaseState interface to define basic functionality.
*/

#nullable enable
using Godot;

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
        Behavior that occurs when the player enters the state
    */
    public void Enter()
    {
        // TODO: implement animation playing and play an animation/sound here.
        return;
    }

    /*
        Exit()
        Behavior that occurs when the player leaves the state
    */
    public void Exit()
    {
        return;
    }

    /*
        ProcessInput(@event)
        Accepts an InputEvent object and processes any relevant inputs for the state and
        input handling logic.
    */
    public IBaseState<Player>? ProcessInput(InputEvent @event)
    {
        return null;
    }

    /*
        ProcessFrame(delta)
        Behavior that occurs on every frame. To be used during the player's _Process layer.
    */
    public IBaseState<Player>? ProcessFrame(double delta)
    {
        return null;
    }

    /*
        ProcessPhysics(delta)
        Behavior that occurs on every frame. To be used during the player's _ProcessPhysics layer.
    */
    public IBaseState<Player>? ProcessPhysics(double delta)
    {
        return null;
    }
}