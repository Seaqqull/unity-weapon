using UnityEngine;


namespace Weapons.Aiming.Following
{
    public class AverageFollower : IFollower
    {
        private Vector3 _direction = Vector3.zero;
        private bool _isValid = true;
        
        public Vector3 FollowDirection { get; private set; }

        
        public AverageFollower(Line[] flow)
        {
            for (var i = 0; i < flow.Length; i++)
                _direction += flow[i].Direction;
            _direction = _direction.normalized;
        }
        

        public bool IsValid() => _isValid;

        public void UpdateDirection(float squaredDistance)
        {
            FollowDirection = _direction;
            _isValid = false;
        }
    }
}