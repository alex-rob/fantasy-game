/*
    IBaseState Interface

    The IBaseState interface is an interface defining how state classes should be implemented.
    This interface should be implemented on the base state class used for any entity
    type implementing a state machine. Each state then inherits from that base state class
    for more detailed implementation of each method.

    Interfaces only define the methods used by the inheriting classes. No fields should
    be defined within this interface, although fields accompanied by getters and setters
    could be included later if refactoring is desired.
*/


#nullable enable
using Godot;

public interface IBaseState<T>
{
    void Enter();
    void Exit();
    IBaseState<T>? ProcessInput(InputEvent @event);
    IBaseState<T>? ProcessFrame(double delta);
    IBaseState<T>? ProcessPhysics(double delta);
}