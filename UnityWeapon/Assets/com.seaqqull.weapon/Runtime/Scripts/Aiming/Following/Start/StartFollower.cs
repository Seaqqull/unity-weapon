using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public class StartFollower : IFollower
  {
    public IReadOnlyList<Line> Flow { get; }
    public Vector3 CurrentDirection { get; private set; }
    public bool CanBeUpdated { get; private set; } = true;


    public StartFollower(IReadOnlyList<Line> flow) =>
      Flow = flow;


    public void UpdateDirection(float squaredDistance)
    {
      if (!CanBeUpdated) return;

      CurrentDirection = Flow[0].Direction;
      CanBeUpdated = false;
    }
  }
}