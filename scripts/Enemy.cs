using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
    [Export] public Hurtbox hurtbox;
    [Export] public float maxHealth = 10;
    [Export] public bool destructable = true;

    public float health;

    public void TakeDamage(Attack attack) {
        health -= attack.damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health <= 0)
        {
            QueueFree();
            GD.Print("Dummy was destroyed by " + attack.damage + " damage.");
        }
        else
        {
            GD.Print(Name + " took " + attack.damage + " damage. Remaining health: " + health);
        }
    }

    public override void _Ready()
    {
        health = maxHealth;
        if (hurtbox is not null) {
            // Connect the hitbox collision event to our handler
            hurtbox.Hit += TakeDamage;
        }
    }
}
