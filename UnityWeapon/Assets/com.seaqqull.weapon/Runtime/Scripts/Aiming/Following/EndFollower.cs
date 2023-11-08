using UnityEngine;

namespace Weapons.Aiming.Following
{
    public class EndFollower : IFollower
    {
        private bool _isValid = true;
        private Line _flow;

        public Vector3 FollowDirection { get; private set; }

        
        public EndFollower(Line[] flow)
        {
            _flow = flow[^1];
        }


        public bool IsValid() => _isValid;

        public void UpdateDirection(float squaredDistance)
        {
            FollowDirection = _flow.Direction;
            _isValid = false;
        }
    }
}