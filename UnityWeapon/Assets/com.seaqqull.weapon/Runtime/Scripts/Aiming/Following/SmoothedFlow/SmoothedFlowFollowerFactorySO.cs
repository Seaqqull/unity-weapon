using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
    [CreateAssetMenu(menuName = "Weapon/Aiming/Following/Smoothed Flow Follower")]
    public class SmoothedFlowFollowerFactorySO : FollowerFactorySO
    {
        public override IFollower Create(IReadOnlyList<Line> flow) =>
            new SmoothedFlowFollower(flow);
    }
}