using UnityEngine;

public interface IInputProvider
{
    bool Enabled { get; set; }
    Vector2 MoveInput { get; }
}
