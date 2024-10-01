using Godot;
using System;

public partial class Main : Node
{
    private World currentWorld;
    private Player player;

    public override void _Ready()
    {
        player = (Player)ResourceLoader.Load<PackedScene>("res://characters/player.tscn").Instantiate();
        currentWorld = (World)ResourceLoader.Load<PackedScene>("res://worlds/world_1.tscn").Instantiate();
        // When the world is ready, move the player to the player spawn
        currentWorld.OnWorldReady += (Node3D playerSpawn) => player.Transform = playerSpawn.Transform;
        AddChild(player);
        AddChild(currentWorld);
        GD.Print("Spawning Complete");
    }

    public override void _Input(InputEvent @event)
    {
        // Basic exit game function prior to UI implementation
        if (@event is InputEventKey eventKey) {
            if (eventKey.Pressed && eventKey.Keycode == Key.Escape) {
                GetTree().Quit();
            }
        }
    }
}
