namespace Weapons.Shooting
{
    public interface IShootingHandler
    {
        bool IsExecutable(IWeapon weapon);
    }
}