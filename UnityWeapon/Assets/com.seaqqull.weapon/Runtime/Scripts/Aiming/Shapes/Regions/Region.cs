using UnityEngine;
using System;


namespace Weapons.Aiming.Shapes.Regions
{
  [Serializable]
  public class Region
  {
    public float Distance = 1;
    [Range(0, 1)] public float Precision = 1;
    public Color Color = Color.black;
  }
}