using System.Collections.Generic;
using UnityEngine;

namespace Weapons.Aiming.Following
{
  public class StartFollower : IFollower
  {
    private readonly Line _flow;

    public Vector3 Direction { get; private set; }
    public bool CanBeRecalculated { get; private set; } = true;


    public StartFollower(IReadOnlyList<Line> flow) =>
      _flow = flow[0];


    public void Recalculate(float squaredDistance)
    {
      if (!CanBeRecalculated) return;

      Direction = _flow.Direction;
      CanBeRecalculated = false;
    }
  }
}