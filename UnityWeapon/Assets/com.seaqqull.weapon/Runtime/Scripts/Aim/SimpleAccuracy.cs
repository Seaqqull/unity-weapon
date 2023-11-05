using System.Collections.Generic;
using Utilities.Methods;
using UnityEngine;
using Weapons.Aiming.Shapes;


namespace Weapons.Aiming
{
    public class SimpleAccuracy : Accuracy
    {
        [Space]
        [SerializeField] private Shape _beginSegment;
        [SerializeField] private Shape _middleSegment;
        [SerializeField] private Shape _endSegment;
        [Header("Visualization")] 
        [SerializeField] [Range(0, 1)] private float _randomDirectionTransparency;
        [Header("Preferences")]
        [SerializeField] private bool _segmentsInfluenceNextPosition;
        [SerializeField] private bool _showRelativeSegments;

        private readonly List<Vector3> _sampleLines = new();
        private Vector3 _startLine, _beginLine, _endLine;

        public Region Begin => _middleSegment.Property;
        public Region End => _endSegment.Property;


        protected override void OnDrawGizmos()
        {
            if (_flow == null) return;

            var rotation = _flow.rotation;
            var position = _flow.position;
            var forward = _flow.forward;

            if (_beginSegment != null)
                _beginSegment.DrawGizmos(position, Vector3.zero, rotation, _shapeColor, _precisionColor);

            for (var i = 1; i < 3; i++)
            {
                var segment = ShapeFromIndex(i);
                segment.DrawGizmos(position, forward, rotation, _shapeColor, _precisionColor);
                position += (forward * segment.Property.Distance);
            }
            
            // Samples.
            for(var i = 1; i < _sampleLines.Count; i++)
            {
                Gizmos.color = ShapeFromIndex(i - 1).Property.Color;
                Gizmos.DrawLine(_sampleLines[i - 1], _sampleLines[i]);
            }
            
            if (!_showRelativeSegments)
                return;

            var precisionShapeColor = new Color(_precisionColor.r, _precisionColor.g, _precisionColor.b, _randomDirectionTransparency);
            var sampleShapeColor = new Color(_shapeColor.r, _shapeColor.g, _shapeColor.b, _randomDirectionTransparency);

            for(var i = 0; i < _sampleLines.Count - 1; i++)
                ShapeFromIndex(i).DrawGizmos(_sampleLines[i], forward, rotation, sampleShapeColor, precisionShapeColor);
        }
       
        private Shape ShapeFromIndex(int index)
        {
            return index switch
            {
                0 => _beginSegment,
                1 => _middleSegment,
                2 => _endSegment,
                _ => null
            };
        }
        
        [ContextMenu("New random direction")]
        protected override void MakeRandomDirection()
        {
            var lines = GetDirection();
            ClearRandomDirection();
            
            foreach (var line in lines)
            {
                _sampleLines.Add(line.From);
            }
            _sampleLines.Add(lines[^1].From + lines[^1].Direction * lines[^1].Length);
        }

        [ContextMenu("Clear random direction")]
        protected override void ClearRandomDirection()
        {
            _sampleLines.Clear();
            _beginLine = Vector3.zero;
            _endLine = Vector3.zero;
        }

        private static float MapAccuracy(float from, float ratio) =>
            ratio >= 0.0f 
                ? ratio >= 0.5f ? ratio.Map(0.5f, 1.0f, from * 2.0f, 1.0f) 
                    : ratio.Map(0.0f, 0.5f, from, from * 2.0f)
                : ratio < -0.5f ? ratio.Map(-0.5f, -1.0f, from * 0.5f, 0.0f) 
                    : ratio.Map(0.0f, -0.5f, from, from * 0.5f);


        public override Line[] GetDirection()
        {
            var lines = new Line[2];
            var rotation = _flow.rotation;
            var position = _flow.position;
            var forward = _flow.forward;
            var fromPosition = _beginSegment == null ? position : _beginSegment.CalculateVector(position, rotation);

            for(var i = 1; i < 3; i++)
            {
                var segment = ShapeFromIndex(i);
                if (!_segmentsInfluenceNextPosition)
                    position += (forward * segment.Property.Distance);
                else
                    position = fromPosition + (forward * segment.Property.Distance);

                var toPosition = segment.CalculateVector(position, rotation);
                var direction = (toPosition - fromPosition);
                lines[i - 1] = new Line(fromPosition, direction.normalized, direction.magnitude);
                fromPosition = toPosition;
            }
            return lines;
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
                _middleSegment.Property.Precision = percent;
                _endSegment.Property.Precision = percent;
                return;
            }
            
            var absoluteAspectRatio = Mathf.Abs(aspectRatio);
            var mappedAccuracy = MapAccuracy(percent, absoluteAspectRatio);
            if (aspectRatio > 0.0f)
            {
                _middleSegment.Property.Precision = percent;
                _endSegment.Property.Precision = mappedAccuracy;
            }
            else
            {
                _middleSegment.Property.Precision = mappedAccuracy;
                _endSegment.Property.Precision = percent;
            }
        }
        
        public void SetGlobalAccuracyFromMiddle(float percent, float aspectRatio)
        {
            aspectRatio = Mathf.Clamp(aspectRatio, -1.0f, 1.0f);
            percent = Mathf.Clamp01(percent);

            if (aspectRatio == 0.0f)
            {
                _middleSegment.Property.Precision = percent;
                _endSegment.Property.Precision = percent;
                return;
            }

            var mappedEndAccuracy = MapAccuracy(percent, aspectRatio);
            var mappedStartAccuracy = MapAccuracy(percent, -aspectRatio);

            _middleSegment.Property.Precision = mappedStartAccuracy;
            _endSegment.Property.Precision = mappedEndAccuracy;
        }
    }
}