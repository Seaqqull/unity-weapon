using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class AverageFollower : IFollower
  {
    private readonly Vector3 _calculatedDirection;

    public Vector3 Direction { get; private set; }
    public bool CanBeRecalculated { get; private set; } = true;


    public AverageFollower(IReadOnlyList<Line> flow)
    {
      for (var i = 0; i < flow.Count; i++)
        _calculatedDirection += flow[i].Direction;
      _calculatedDirection /= flow.Count;

      Recalculate(0.0f);
    }


    public void Recalculate(float squaredDistance)
    {
      if (!CanBeRecalculated) return;

      Direction = _calculatedDirection;
      CanBeRecalculated = false;
    }
  }
}