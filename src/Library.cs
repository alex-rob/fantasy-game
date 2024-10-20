using Godot;

namespace Library;


public static class Vectors
{
    // Get the current 2D (X,Z) input direction vector from Godot's Input.GetVector function.
    public static Vector2 GetCurrentInputDirection()
    {
        return Input.GetVector("move_left", "move_right", "move_forward", "move_back");
    }

    public static Vector3 MoveTowardsXZ(Vector3 source, Vector3 destination, float rate)
    {
        Vector3 temp = source + ((destination - source) * rate);
        return new Vector3(temp.X, source.Y, temp.Z);
    }
}