using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class AverageFollower : IFollower
  {
    private readonly Vector3 _direction;

    private bool _isValid = true;

    public Vector3 FollowDirection { get; private set; }


    public AverageFollower(IReadOnlyList<Line> flow)
    {
      for (var i = 0; i < flow.Count; i++)
        _direction += flow[i].Direction;
      _direction /= flow.Count;
    }


    public bool IsValid() => _isValid;

    public void UpdateDirection(float squaredDistance)
    {
      FollowDirection = _direction;
      _isValid = false;
    }
  }
}