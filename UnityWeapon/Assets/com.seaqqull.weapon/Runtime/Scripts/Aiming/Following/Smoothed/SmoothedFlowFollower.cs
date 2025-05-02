using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class SmoothedFlowFollower : IFollower
  {
    private readonly IReadOnlyList<Line> _flow;
    private readonly float[] _distances;

    public Vector3 Direction { get; private set; }
    public bool CanBeRecalculated { get; private set; } = true;


    public SmoothedFlowFollower(IReadOnlyList<Line> flow)
    {
      _distances = new float[flow.Count + 1];
      _flow = flow;

      for (var i = 0; i < _flow.Count; i++)
        _distances[i + 1] = _distances[i] + _flow[i].SquaredLength;
    }


    public void Recalculate(float squaredDistance)
    {
      for (var i = 0; i < _distances.Length - 1; i++)
      {
        if (squaredDistance >= _distances[i]) continue;

        var lerp = Mathf.InverseLerp(_distances[i - 1], _distances[i], squaredDistance);
        Direction = Vector3.Lerp(_flow[i - 1].Direction, _flow[i].Direction, lerp).normalized;
        return;
      }
      if (!CanBeRecalculated) return;

      Direction = _flow[^1].Direction;
      CanBeRecalculated = false;
    }
  }
}