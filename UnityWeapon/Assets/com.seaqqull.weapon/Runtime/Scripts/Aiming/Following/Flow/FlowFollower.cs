using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class FlowFollower : IFollower
  {
    private bool _isValid = true;
    private float[] _distances;
    private IReadOnlyList<Line> _flow;

    public Vector3 FollowDirection { get; private set; }


    public FlowFollower(IReadOnlyList<Line> flow)
    {
      _distances = new float[flow.Count + 1];
      _flow = flow;

      for (var i = 0; i < _flow.Count; i++)
        _distances[i + 1] = _distances[i] + _flow[i].SquaredLength;
    }


    public bool IsValid() => _isValid;

    public void UpdateDirection(float squaredDistance)
    {
      for (var i = 0; i < _distances.Length; i++)
      {
        if (squaredDistance >= _distances[i]) continue;

        FollowDirection = _flow[i - 1].Direction;
        return;
      }
      if (!_isValid) return;

      FollowDirection = _flow[^1].Direction;
      _isValid = false;
    }
  }
}