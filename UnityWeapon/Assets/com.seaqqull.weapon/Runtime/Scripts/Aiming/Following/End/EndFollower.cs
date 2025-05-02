using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class EndFollower : IFollower
  {
    public IReadOnlyList<Line> Flow { get; }
    public Vector3 CurrentDirection { get; private set; }
    public bool CanBeUpdated { get; private set; } = true;


    public EndFollower(IReadOnlyList<Line> flow)
    {
      Flow = flow;
      UpdateDirection(0.0f);
    }


    public void UpdateDirection(float squaredDistance)
    {
      if (!CanBeUpdated) return;

      CurrentDirection = Flow[^1].Direction;
      CanBeUpdated = false;
    }
  }
}