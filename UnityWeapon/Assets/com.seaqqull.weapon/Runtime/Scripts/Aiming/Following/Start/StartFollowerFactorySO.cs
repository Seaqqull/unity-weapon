using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
    [CreateAssetMenu(menuName = "Weapon/Aiming/Following/Start Follower")]
    public class StartFollowerFactorySO : FollowerFactorySO
    {
        public override IFollower Create(IReadOnlyList<Line> flow) =>
            new StartFollower(flow);
    }
}