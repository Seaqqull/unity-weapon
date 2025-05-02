using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
    [CreateAssetMenu(menuName = "Weapon/Aiming/Following/Average Follower")]
    public class AverageFollowerFabricSO : FollowerFabricSO
    {
        public override IFollower Create(IReadOnlyList<Line> flow) =>
            new AverageFollower(flow);
    }
}