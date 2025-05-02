using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class FlowFollower : IFollower
  {
    private readonly float[] _distances;

    public IReadOnlyList<Line> Flow { get; }
    public Vector3 CurrentDirection { get; private set; }
    public bool CanBeUpdated { get; private set; } = true;


    public FlowFollower(IReadOnlyList<Line> flow)
    {
      _distances = new float[flow.Count + 1];
      Flow = flow;

      for (var i = 0; i < Flow.Count; i++)
        _distances[i + 1] = _distances[i] + Flow[i].SquaredLength;
    }


    public void UpdateDirection(float squaredDistance)
    {
      for (var i = 0; i < _distances.Length; i++)
      {
        if (squaredDistance >= _distances[i]) continue;

        CurrentDirection = Flow[i - 1].Direction;
        return;
      }
      if (!CanBeUpdated) return;

      CurrentDirection = Flow[^1].Direction;
      CanBeUpdated = false;
    }
  }
}