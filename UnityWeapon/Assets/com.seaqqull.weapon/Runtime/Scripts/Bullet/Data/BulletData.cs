using UnityEngine;
using Weapons.Aiming.Following;

namespace Weapons.Bullets
{
  public class BulletData : IBulletData
  {
    public IFollowerFabric Follower { get; }
    public LayerMask TargetMask { get; }
    public bool LookRotation { get; }
    public int Damage { get; }
    public float Speed { get; }
    public float Range { get; }
  }
}