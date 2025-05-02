using Weapons.Aiming.Following;

namespace Weapons.Bullets
{
  public interface IBulletSetup
  {
    IBulletData Data { get; }
    IFollowerFactoryProvider Follow { get; }
  }
}