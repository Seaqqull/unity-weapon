namespace Weapons.Shooting
{
    public interface IShootingMode
    {
        int BulletsToPerformShot { get; }
        float TimeBetweenShot { get; }

        void Perform(IWeapon weapon);
        bool IsExecutable(IWeapon weapon);
    }
}