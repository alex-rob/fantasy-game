using Godot;
using System;

public partial class World : Node
{
    [Export] public Node3D playerSpawn;

    [Signal] public delegate void OnWorldReadyEventHandler(Node3D playerSpawn);

    public override void _Ready()
    {
        EmitSignal(SignalName.OnWorldReady, playerSpawn);
        GD.Print("World Ready");
    }
}