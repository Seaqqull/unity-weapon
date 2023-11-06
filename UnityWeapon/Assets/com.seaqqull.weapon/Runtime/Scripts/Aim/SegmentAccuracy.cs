using System.Collections.Generic;
using Weapons.Aiming.Shapes;
using UnityEngine;


namespace Weapons.Aiming
{
    public class SegmentAccuracy : Accuracy
    {
        [SerializeField] private Shape _beginSegment;
        [SerializeField] private Shape[] _segments;
        [Header("Visualization")] 
        [SerializeField] [Range(0, 1)] private float _randomDirectionTransparency;
        [Header("Preferences")]
        [SerializeField] private bool _segmentsInfluenceNextPosition;
        [SerializeField] private bool _showRelativeSegments;

        private readonly List<Vector3> _sampleLines = new();


        protected override void OnDrawGizmos()
        {
            if (_flow == null || _segments == null) return;

            var rotation = _flow.rotation;
            var position = _flow.position;
            var forward = _flow.forward;

            if (_beginSegment != null)
                _beginSegment.DrawGizmos(position, Vector3.zero, rotation, _shapeColor, _precisionColor);
            
            foreach (var segment in _segments)
            {
                segment.DrawGizmos(position, forward, rotation, _shapeColor, _precisionColor);
                position += (forward * segment.Property.Distance);
            }

            // Samples.
            for(var i = 1; i < _sampleLines.Count; i++)
            {
                Gizmos.color = _segments[i - 1].Property.Color;
                Gizmos.DrawLine(_sampleLines[i - 1], _sampleLines[i]);
            }
            if (!_showRelativeSegments)
                return;

            var precisionShapeColor = new Color(_precisionColor.r, _precisionColor.g, _precisionColor.b, _randomDirectionTransparency);
            var sampleShapeColor = new Color(_shapeColor.r, _shapeColor.g, _shapeColor.b, _randomDirectionTransparency);

            for(var i = 0; i < _sampleLines.Count - 1; i++)
                _segments[i].DrawGizmos(_sampleLines[i], forward, rotation, sampleShapeColor, precisionShapeColor);
        }


        [ContextMenu("Create sample direction")]
        protected override void MakeRandomDirection()
        {
            var lines = CreateDirection();
            
            ClearRandomDirection();
            foreach (var line in lines)
            {
                _sampleLines.Add(line.From);
            }
            _sampleLines.Add(lines[^1].From + lines[^1].Direction * lines[^1].Length);
        }

        [ContextMenu("Clear sample direction")]
        protected override void ClearRandomDirection()
        {
            _sampleLines.Clear();
        }


        public override Line[] CreateDirection()
        {
            var lines = new Line[_segments.Length];
            var rotation = _flow.rotation;
            var position = _flow.position;
            var forward = _flow.forward;
            var fromPosition = _beginSegment == null ? position : _beginSegment.CalculateVector(position, rotation);

            for(var i = 0; i < _segments.Length; i++)
            {
                if (!_segmentsInfluenceNextPosition)
                    position += (forward * _segments[i].Property.Distance);
                else
                    position = fromPosition + (forward * _segments[i].Property.Distance);

                var toPosition = _segments[i].CalculateVector(position, rotation);
                var direction = (toPosition - fromPosition);
                lines[i] = new Line(fromPosition, direction.normalized, direction.magnitude);
                fromPosition = toPosition;
            }
            return lines;
        }
    }
}