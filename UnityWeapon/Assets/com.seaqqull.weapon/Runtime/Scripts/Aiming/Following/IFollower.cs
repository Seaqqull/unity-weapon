using UnityEngine;


namespace Weapons.Aiming.Following
{
    public interface IFollower
    {
        Vector3 Direction { get; }
        bool CanBeRecalculated { get; }

        void Recalculate(float squaredDistance);
    }
}