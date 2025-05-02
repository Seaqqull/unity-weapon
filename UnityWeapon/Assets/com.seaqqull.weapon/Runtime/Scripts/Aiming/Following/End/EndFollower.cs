using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class EndFollower : IFollower
  {
    private readonly Line _flow;

    public Vector3 Direction { get; private set; }
    public bool CanBeRecalculated { get; private set; } = true;


    public EndFollower(IReadOnlyList<Line> flow)
    {
      _flow = flow[^1];

      Recalculate(0.0f);
    }


    public void Recalculate(float squaredDistance)
    {
      if (!CanBeRecalculated) return;

      Direction = _flow.Direction;
      CanBeRecalculated = false;
    }
  }
}