namespace Weapons.Aiming.Following
{
  public interface IFollowerFactoryProvider
  {
    IFollowerFabric FollowerFactory { get; }
  }
}