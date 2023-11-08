using UnityEngine;


namespace Weapons.Aiming.Following
{
    public class SmoothedFlowFollower : IFollower
    {
        private bool _isValid = true;
        private float[] _distances;
        private Line[] _flow;

        public Vector3 FollowDirection { get; private set; }

        
        public SmoothedFlowFollower(Line[] flow)
        {
            _distances = new float[flow.Length + 1];
            _flow = flow;

            for (var i = 0; i < _flow.Length; i++)
                _distances[i + 1] = _distances[i] + _flow[i].SquaredLength;
        }


        public bool IsValid() => _isValid;

        public void UpdateDirection(float squaredDistance)
        {
            for (var i = 0; i < _distances.Length - 1; i++)
            {
                if (squaredDistance >= _distances[i]) continue;
                
                var lerp = Mathf.InverseLerp(_distances[i - 1], _distances[i], squaredDistance);
                FollowDirection = Vector3.Lerp(_flow[i - 1].Direction, _flow[i].Direction, lerp).normalized;
                return;
            }
            if (!_isValid) return;
 
            FollowDirection = _flow[^1].Direction;
            _isValid = false;
        }
    }
}