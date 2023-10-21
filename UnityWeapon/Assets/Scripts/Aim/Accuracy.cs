using Random = UnityEngine.Random;
using Weapons.Utility;
using UnityEngine;
using System;


namespace Weapons.Aiming
{
    public class Accuracy : BaseMonoBehaviour
    {
        public record Line(Vector3 From, Vector3 Direction);
        [Serializable]
        public class Circle
        {
            public float Radius = 1;
            [Range(0, 1)] public float Accuracy = 1;
        }
        [Serializable]
        private class CircleVisualization
        {
            public Color ColorRadius = new(0, 1, 0, 1);
            public Color ColorAccuracy = new(1, 1, 0, 1);
        }


        [SerializeField] private Transform _parent;
        [SerializeField] private float _distance = 1.0f;
        [Space]
        [SerializeField] private Circle _begin;
        [SerializeField] private Circle _end;
        [Header("Visualization")]
        [SerializeField] private int _edgeCount = 10;
        [SerializeField] private CircleVisualization _beginVisualization;
        [SerializeField] private CircleVisualization _endVisualization;

#if UNITY_EDITOR
        private Vector3 beginLine, endLine;
#endif

        public Circle Begin => _begin;
        public Circle End => _end;


        private void OnDrawGizmos()
        {
            if (_parent == null) return;

            var errorFlag =
                DrawCircleGizmo(_parent.position, _parent.rotation, radius: _begin.Radius, color: _beginVisualization.ColorRadius) &&
                DrawCircleGizmo(_parent.position, _parent.rotation, radius: (_begin.Radius * (1 - _begin.Accuracy)), color: _beginVisualization.ColorAccuracy);
            if (!errorFlag)
                Debug.LogWarning($"Wrong parameters ({nameof(Circle.Radius)}, {nameof(_edgeCount)})" +
                                 " in begin circle, so it can't be displayed properly.", GameObj);

            errorFlag =
                DrawCircleGizmo(_parent.position, _parent.rotation, offset: (_parent.forward * _distance), radius: _end.Radius, color: _endVisualization.ColorRadius) &&
                DrawCircleGizmo(_parent.position, _parent.rotation, offset: (_parent.forward * _distance), radius: (_end.Radius * (1 - _end.Accuracy)), color: _endVisualization.ColorAccuracy);
            if (!errorFlag)
                Debug.LogWarning($"Wrong parameters ({nameof(Circle.Radius)}, {nameof(_edgeCount)})" +
                                 " in begin circle, so it can't be displayed properly.", GameObj);

#if UNITY_EDITOR
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(beginLine, endLine);
#endif
            bool DrawCircleGizmo(Vector3 position, Quaternion rotation, Vector3 offset = new(), float radius = 0, Color color = new())
            {
                if (radius <= 0 && _edgeCount < 3) return false;

                var positionWithOffset = position + offset;
                var originalColor = Gizmos.color;
                Gizmos.color = color;

                var vertices = CreateCircle(positionWithOffset, rotation, radius, _edgeCount);
                for (var i = 0; i < vertices.Length - 1; i++)
                    Gizmos.DrawLine(vertices[i], vertices[i + 1]);

                Gizmos.DrawLine(vertices[^1], vertices[0]);
                Gizmos.color = originalColor;

                return true;
            }
        }


#if UNITY_EDITOR
        [ContextMenu("New random direction")]
        private void MakeRandomDirection()
        {
            beginLine = CalculateVector(
                _parent.position, 
                _parent.rotation, 
                _begin.Radius * (1 - _begin.Accuracy));
            endLine = CalculateVector(
                _parent.position + (_parent.forward * _distance), 
                _parent.rotation, 
                _end.Radius * (1 - _end.Accuracy));
        }
#endif


        private static float MapAccuracy(float from, float ratio) =>
            ratio >= 0.0f 
                ? ratio >= 0.5f ? ratio.Map(0.5f, 1.0f, from * 2.0f, 1.0f) 
                    : ratio.Map(0.0f, 0.5f, from, from * 2.0f)
                : ratio < -0.5f ? ratio.Map(-0.5f, -1.0f, from * 0.5f, 0.0f) 
                    : ratio.Map(0.0f, -0.5f, from, from * 0.5f);

        private static Vector3 CalculateVector(Vector3 position, Quaternion rotation, float radius)
        {
            var rndPos = Random.insideUnitCircle * radius;
            var shift = new Vector3(rndPos.x, rndPos.y, 0);

            return position + (rotation * shift);
        }

        private static Vector3[] CreateCircle(Vector3 position, Quaternion rotation, float radius, int edgeCount)
        {
            var angleStep = (360.0f / edgeCount);
            var vertices = new Vector3[edgeCount];
            
            for (var i = 0; i < edgeCount; i++)
            {
                var angle = i * angleStep;
                var arcPoint = new Vector3(
                    Mathf.Sin(Mathf.Deg2Rad *  angle) * radius, 
                    Mathf.Cos(Mathf.Deg2Rad *  angle) * radius, 
                    0);

                vertices[i] = position + rotation * arcPoint;
            }

            return vertices;
        }

        public Line GetDirectionVector()
        {
            var begin = CalculateVector(
                _parent.position, 
                _parent.rotation, 
                _begin.Radius * (1 - _begin.Accuracy));
            var end = CalculateVector(
                _parent.position + (_parent.forward * _distance), 
                _parent.rotation, 
                _end.Radius * (1 - _end.Accuracy));

            return new Line(begin, (end - begin).normalized);
        }

        /// <summary>
        /// Sets begin and end accuracy.
        /// </summary>
        /// <param name="percent"> Global accuracy value.</param>
        /// <param name="aspectRatio"> Velue between 0 and 2. Relation between begin and end accuracy.</param>
        public void SetGlobalAccuracy(float percent, float aspectRatio)
        {
            aspectRatio = Mathf.Clamp(aspectRatio, -1.0f, 1.0f);
            percent = Mathf.Clamp01(percent);

            if (aspectRatio == 0.0f)
            {
                _begin.Accuracy = percent;
                _end.Accuracy = percent;
                return;
            }
            
            var absoluteAspectRatio = Mathf.Abs(aspectRatio);
            var mappedAccuracy = MapAccuracy(percent, absoluteAspectRatio);
            if (aspectRatio > 0.0f)
            {
                _begin.Accuracy = percent;
                _end.Accuracy = mappedAccuracy;
            }
            else
            {
                _begin.Accuracy = mappedAccuracy;
                _end.Accuracy = percent;
            }
        }
        
        public void SetGlobalAccuracyFromMiddle(float percent, float aspectRatio)
        {
            aspectRatio = Mathf.Clamp(aspectRatio, -1.0f, 1.0f);
            percent = Mathf.Clamp01(percent);

            if (aspectRatio == 0.0f)
            {
                _begin.Accuracy = percent;
                _end.Accuracy = percent;
                return;
            }

            var mappedEndAccuracy = MapAccuracy(percent, aspectRatio);
            var mappedStartAccuracy = MapAccuracy(percent, -aspectRatio);

            _begin.Accuracy = mappedStartAccuracy;
            _end.Accuracy = mappedEndAccuracy;
        }
    }
}
