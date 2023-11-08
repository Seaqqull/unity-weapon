using UnityEngine;


namespace Weapons.Aiming.Following
{
    public class FlowFollower : IFollower
    {
        private bool _isValid = true;
        private float[] _distances;
        private Line[] _flow;

        public Vector3 FollowDirection { get; private set; }

        
        public FlowFollower(Line[] flow)
        {
            _distances = new float[flow.Length + 1];
            _flow = flow;

            for (var i = 0; i < _flow.Length; i++)
                _distances[i + 1] = _distances[i] + _flow[i].SquaredLength;
        }


        public bool IsValid() => _isValid;

        public void UpdateDirection(float squaredDistance)
        {
            for (var i = 0; i < _distances.Length; i++)
            {
                if (squaredDistance >= _distances[i]) continue;
                
                FollowDirection = _flow[i - 1].Direction;
                return;
            }
            if (!_isValid) return;
 
            FollowDirection = _flow[^1].Direction;
            _isValid = false;
        }
    }
}