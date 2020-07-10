using UnityEngine;


namespace Weapons.Aiming
{
    public class Accuracy : BaseMonoBehaviour
    {
        [System.Serializable]
        public class Circle
        {
            public float Radius = 1;
            public Color ColorRadius = new Color(0, 1, 0, 1);
            [Range(0, 1)] public float Accuracy = 1;
            public Color ColorAccuracy = new Color(1, 1, 0, 1);
        }


        [SerializeField] private Transform _parent;
        [SerializeField] private int _edgeCount = 10;
        [SerializeField] private float _distance = 1;
        [SerializeField] private Circle _begin;
        [SerializeField] private Circle _end;

        private Color _originalColor;

        public Circle End
        {
            get { return this._end; }
        }
        public Circle Begin
        {
            get { return this._begin; }
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_parent == null) return;

            bool errorFlag;

            // Inner            
            errorFlag =
                DrawCircleGizmo
                (_parent.position, _parent.rotation, radius: _begin.Radius, color: _begin.ColorRadius) &&
                DrawCircleGizmo
                (_parent.position, _parent.rotation, radius: (_begin.Radius * (1 - _begin.Accuracy)), color: _begin.ColorAccuracy);
            if (!errorFlag)
            {
                Debug.Log("There are some wrong parameters in begin circle, so it can't be displayed properly.", GameObj);
            }


            // Outer
            errorFlag =
                DrawCircleGizmo
                (_parent.position, _parent.rotation, offset: (_parent.forward * _distance), radius: _end.Radius, color: _end.ColorRadius) &&
                DrawCircleGizmo
                (_parent.position, _parent.rotation, offset: (_parent.forward * _distance), radius: (_end.Radius * (1 - _end.Accuracy)), color: _end.ColorAccuracy);

            if (!errorFlag)
            {
                Debug.Log("There are some wrong parameters in end circle, so it can't be displayed properly.", GameObj);
            }
        }
#endif


        private Vector3 CalculateVector(Vector3 position, Quaternion rotation, float radius)
        {
            Vector3 shift;
            Vector2 rndPos = Random.insideUnitCircle * radius;

            shift = new
                Vector3(rndPos.x, rndPos.y, 0);

            return position + (rotation * shift);
        }

        private Vector3[] CreateCircle(Vector3 position, Quaternion rotation, float radius, int edgeCount)
        {
            Vector3[] vertecies = new Vector3[edgeCount];

            int cnter = 0;

            for (float i = 0; i < 360.0f; i += (360.0f / edgeCount))
            {
                Vector3 tmp = new Vector3(Mathf.Sin(Mathf.Deg2Rad * i) * radius, Mathf.Cos(Mathf.Deg2Rad * i) * radius, 0);
                tmp = rotation * tmp;

                vertecies[cnter++] = position + tmp;
            }

            return vertecies;
        }

        private bool DrawCircleGizmo(Vector3 position, Quaternion rotation, Vector3 offset = new Vector3(), float radius = 0, Color color = new Color())
        {
            if ((radius <= 0) &&
                (_edgeCount < 3)) return false;

            Vector3 positionWithOffset = position + offset;
            _originalColor = Gizmos.color;
            Gizmos.color = color;

            Vector3[] vertecies = CreateCircle(positionWithOffset, rotation, radius, _edgeCount);

            for (int i = 0; i < vertecies.Length - 1; i++)
            {
                Gizmos.DrawLine(vertecies[i], vertecies[i + 1]);
            }
            Gizmos.DrawLine(vertecies[vertecies.Length - 1], vertecies[0]);

            Gizmos.color = _originalColor;

            return true;
        }


        public void SetEndAccuracy(float percent)
        {
            if ((percent < 0.0f) ||
                (percent > 1.0f)) return;

            _end.Accuracy = percent;
        }

        public void SetBeginAccuracy(float percent)
        {
            if ((percent < 0.0f) ||
                (percent > 1.0f)) return;

            _begin.Accuracy = percent;
        }

        public System.Tuple<Vector3, Vector3> GetDirectedVector()
        {
            Vector3 begin = CalculateVector(_parent.position, _parent.localRotation, (_begin.Radius * (1 - _begin.Accuracy)));
            Vector3 end = CalculateVector(_parent.position + (_parent.forward * _distance), _parent.localRotation, (_end.Radius * (1 - _end.Accuracy)));

            return new System.Tuple<Vector3, Vector3>(begin, end);
        }

        /// <summary>
        /// Sets begin and end accuracy.
        /// </summary>
        /// <param name="percent"> Global accuracy value.</param>
        /// <param name="aspectRatio"> Velue between 0 and 2. Relation between begin and end accuracy.</param>
        public void SetGlobalAccuracy(float percent, float aspectRatio)
        {
            if ((percent < 0.0f ||
                 percent > 1.0f) ||
                (aspectRatio < 0.0f ||
                 aspectRatio > 2.0f)) return;

            SetBeginAccuracy(percent * (1 - (aspectRatio / 2.0f)));
            SetEndAccuracy(percent * (aspectRatio / 2.0f));
        }
    }
}
