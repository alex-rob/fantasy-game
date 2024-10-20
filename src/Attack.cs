using Godot;
using System;

public partial class Attack : Area3D
{
    public float damage;
    public float knockback;
    public float cooldown;

    // Create an instance of attack with provided properties of damage, knockback, and cooldown.
    public Attack(float atkDamage = 1f, float atkKnockback = 0f, float atkCooldown = 0.5f)
    {
        damage = atkDamage;
        knockback = atkKnockback;
        cooldown = atkCooldown;
    }
}