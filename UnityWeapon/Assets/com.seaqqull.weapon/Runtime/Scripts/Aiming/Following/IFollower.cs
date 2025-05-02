using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
    public interface IFollower
    {
        IReadOnlyList<Line> Flow { get; }
        Vector3 CurrentDirection { get; }
        bool CanBeUpdated { get; }

        void UpdateDirection(float squaredDistance);
    }
}