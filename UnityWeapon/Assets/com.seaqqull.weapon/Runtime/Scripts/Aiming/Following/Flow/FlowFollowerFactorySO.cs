using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
    [CreateAssetMenu(menuName = "Weapon/Aiming/Following/Flow Follower")]
    public class FlowFollowerFactorySO : FollowerFactorySO
    {
        public override IFollower Create(IReadOnlyList<Line> flow) =>
            new FlowFollower(flow);
    }
}