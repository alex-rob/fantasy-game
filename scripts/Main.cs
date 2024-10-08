using Godot;
using System;

public partial class Main : Node
{
    private World _currentWorld;
    private Player _player;
    private bool _mouseLockedHidden = true;

    public override void _Ready()
    {
        _player = (Player)ResourceLoader.Load<PackedScene>("res://characters/player.tscn").Instantiate();
        _currentWorld = (World)ResourceLoader.Load<PackedScene>("res://worlds/world_1.tscn").Instantiate();
        // When the world is ready, move the player to the player spawn
        _currentWorld.OnWorldReady += (Node3D playerSpawn) => _player.Transform = playerSpawn.Transform;
        AddChild(_player);
        AddChild(_currentWorld);
        GD.Print("Spawning Complete");

        if (_mouseLockedHidden)
        {
            Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
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
