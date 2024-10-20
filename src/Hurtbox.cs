using Godot;
using System;

public partial class Hurtbox : Area3D
{
    [Signal] public delegate void HitEventHandler(Attack attack);

    public override void _Ready()
    {
        AreaEntered += HitRegistered;
    }

    public void HitRegistered(Area3D area)
    {
        if (area is Attack) {
            EmitSignal(SignalName.Hit, area);
        }
    }
}