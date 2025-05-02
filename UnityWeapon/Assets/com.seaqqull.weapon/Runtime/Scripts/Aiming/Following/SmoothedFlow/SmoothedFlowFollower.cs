using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class SmoothedFlowFollower : IFollower
  {
    private readonly float[] _distances;

    public IReadOnlyList<Line> Flow { get; }
    public Vector3 CurrentDirection { get; private set; }
    public bool CanBeUpdated { get; private set; } = true;


    public SmoothedFlowFollower(IReadOnlyList<Line> flow)
    {
      _distances = new float[flow.Count + 1];
      Flow = flow;

      for (var i = 0; i < Flow.Count; i++)
        _distances[i + 1] = _distances[i] + Flow[i].SquaredLength;
    }


    public void UpdateDirection(float squaredDistance)
    {
      for (var i = 0; i < _distances.Length - 1; i++)
      {
        if (squaredDistance >= _distances[i]) continue;

        var lerp = Mathf.InverseLerp(_distances[i - 1], _distances[i], squaredDistance);
        CurrentDirection = Vector3.Lerp(Flow[i - 1].Direction, Flow[i].Direction, lerp).normalized;
        return;
      }
      if (!CanBeUpdated) return;

      CurrentDirection = Flow[^1].Direction;
      CanBeUpdated = false;
    }
  }
}