using System.Collections.Generic;


namespace Weapons.Aiming.Following
{
  public interface IFollowerFabric
  {
    IFollower Create(IReadOnlyList<Line> flow);
  }
}