using UnityEngine;


namespace Weapons.Aiming.Following
{
    public interface IFollower
    {
        Vector3 FollowDirection { get; }


        bool IsValid();
        void UpdateDirection(float squaredDistance);
    }
}