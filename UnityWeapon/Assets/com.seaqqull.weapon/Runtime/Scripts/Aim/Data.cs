using UnityEngine;
using System;


namespace Weapons.Aiming
{
    public record Line(Vector3 From, Vector3 Direction, float Length);
    
    [Serializable]
    public class Region
    {
        public float Distance = 1;
        [Range(0, 1)] public float Precision = 1;
        public Color Color = Color.black;
    }

    [Serializable]
    public class CircleRegion : Region
    {
        public float Radius = 1;
    }

    [Serializable]
    public class RectangleRegion : Region
    {
        public float Width = 1;
        public float Height = 1;
    }
}