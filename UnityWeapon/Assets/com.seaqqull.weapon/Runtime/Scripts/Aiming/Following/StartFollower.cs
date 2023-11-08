using UnityEngine;


namespace Weapons.Aiming.Following
{
    public class StartFollower : IFollower
    {
        private bool _isValid = true;
        private Line _flow;

        public Vector3 FollowDirection { get; private set; }


        public StartFollower(Line[] flow)
        {
            _flow = flow[0];
        }
        

        public bool IsValid() => _isValid;

        public void UpdateDirection(float squaredDistance)
        {
            FollowDirection = _flow.Direction;
            _isValid = false;
        }
    }
}