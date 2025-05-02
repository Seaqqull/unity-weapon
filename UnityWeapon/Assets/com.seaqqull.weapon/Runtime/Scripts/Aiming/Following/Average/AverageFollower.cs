using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class AverageFollower : IFollower
  {
    public IReadOnlyList<Line> Flow { get; }
    private readonly Vector3 _calculatedDirection;

    public Vector3 CurrentDirection { get; private set; }
    public bool CanBeRecalculated { get; private set; } = true;


    public AverageFollower(IReadOnlyList<Line> flow)
    {
      Flow = flow;

      for (var i = 0; i < flow.Count; i++)
        _calculatedDirection += flow[i].Direction;
      _calculatedDirection /= flow.Count;

      UpdateDirection(0.0f);
    }


    public void UpdateDirection(float squaredDistance)
    {
      if (!CanBeRecalculated) return;

      CurrentDirection = _calculatedDirection;
      CanBeRecalculated = false;
    }
  }
}