using UnityEngine;


namespace Weapons.Aiming
{
    public record Line(Vector3 From, Vector3 Direction, float SquaredLength);
}