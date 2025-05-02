using System.Collections.Generic;
using UnityEngine;


namespace Weapons.Aiming.Following
{
  public abstract class FollowerFabricSO : ScriptableObject, IFollowerFabric
  {
    public abstract IFollower Create(IReadOnlyList<Line> flow);
  }
}