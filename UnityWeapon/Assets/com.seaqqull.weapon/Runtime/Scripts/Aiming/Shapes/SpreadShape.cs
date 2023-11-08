using Random = UnityEngine.Random;
using Utilities.Methods;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;


namespace Weapons.Aiming.Shapes
{
    public enum SpreadType { Random, PlaintSequence, RandomSequence}
    
    [Serializable]
    public class SpreadShape : IShape
    {
        [SerializeField] private Shape Shape;
        [Space]
        [SerializeField] private float VisualizationSize = 0.1f;
        [Space] 
        [SerializeField] private SpreadType SpreadType;
        [SerializeField] private bool RandomOrder;
        [SerializeField] private bool AlignWithAccuracy;
        [Space]
        [SerializeField] private Vector2[] PlaintSequence;
        /// <summary>
        /// Horizontal - [x, y], Vertical - [z, w].
        /// </summary>
        [Tooltip("Horizontal - [x, y], Vertical - [z, w].")]
        [SerializeField] private SpreadArea[] RandomSequence;

        private int _dedicatedSequenceIndex;

        public Region Property => Shape.Property;
        
        
        public void DrawGizmos(Vector3 position, Vector3 forward, Quaternion rotation, Color shapeColor, Color precisionColor)
        {
            Shape.DrawGizmos(position, forward, rotation, shapeColor, precisionColor);

#if UNITY_EDITOR
            Handles.color = Shape.Property.Color;
#endif
            Gizmos.color = Shape.Property.Color;
            var startPosition = position + (forward * Shape.Property.Distance);

            for (var i = 0; SpreadType == SpreadType.PlaintSequence && i < PlaintSequence.Length; i++)
            {
                var spreadPosition = startPosition + (rotation * PlaintSequence[i]);
                Gizmos.DrawWireSphere(spreadPosition, VisualizationSize);
#if UNITY_EDITOR
                Handles.Label(spreadPosition, $"{i}");
#endif
            }
            
            // Draw square range.
            for (var i = 0; SpreadType == SpreadType.RandomSequence && i < RandomSequence.Length; i++)
            {
                var area = RandomSequence[i];
                var points = new[]
                {
                    startPosition + (rotation * (area.Shift + new Vector3(area.Area.x, area.Area.z))),
                    startPosition + (rotation * (area.Shift + new Vector3(area.Area.y, area.Area.z))),
                    startPosition + (rotation * (area.Shift + new Vector3(area.Area.y, area.Area.w))),
                    startPosition + (rotation * (area.Shift + new Vector3(area.Area.x, area.Area.w)))
                };
                Gizmos.DrawLineList(new Vector3[]
                {
                    points[0], points[1], points[1], points[2], points[2], points[3], points[3], points[0],
                });
#if UNITY_EDITOR
                for(var j = 0; j < points.Length; j++)
                    Handles.Label(points[j], $"{i}");
#endif
            }
        }

        public Vector3 CalculateVector()
        {
            switch (SpreadType)
            {
                case SpreadType.Random:
                    return Shape.CalculateVector();
                case SpreadType.PlaintSequence:
                    var plaintPoint = PlaintSequence[NextIndex()];

                    return AlignWithAccuracy ? Clamp(plaintPoint) : plaintPoint;
                case SpreadType.RandomSequence:
                    var range = RandomSequence[NextIndex()];
                    var randomPoint = range.Shift + new Vector3(
                        Random.value.Map(0.0f, 1.0f, range.Area.x, range.Area.y), 
                        Random.value.Map(0.0f, 1.0f, range.Area.z, range.Area.w));

                    return AlignWithAccuracy ? Clamp(randomPoint) : randomPoint;
                default:
                    return Shape.CalculateVector();
            }

            int NextIndex()
            {
                if (RandomOrder)
                {
                    var bound = SequenceBound();
                    return (int) Mathf.Clamp(0, Random.value.Map(0, 1, 0, bound), bound - 1);
                }

                if (_dedicatedSequenceIndex >= SequenceBound())
                    _dedicatedSequenceIndex = 0;
                return _dedicatedSequenceIndex++;
            }

            int SequenceBound()
            {
                return SpreadType switch
                {
                    SpreadType.PlaintSequence => PlaintSequence.Length,
                    SpreadType.RandomSequence => RandomSequence.Length,
                    _ => 1
                };
            }
        }

        public Vector3 Clamp(Vector3 point)
        {
            return Shape.Clamp(point);
        }

        public Vector3 CalculateVector(Vector3 position, Quaternion rotation)
        {
            return position + (rotation * CalculateVector());
        }

        public static implicit operator Shape(SpreadShape spread) => spread.Shape;
    }
}