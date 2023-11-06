using Weapon.Computing;


namespace Weapons.Shooting
{
    public abstract class ShootingMode : ComputingHandler, IShootingMode
    {
        public int BulletsToPerformShot => 1;
        public float TimeBetweenShot => 0.1f;


        public abstract bool IsExecutable(IWeapon weapon);


        public virtual void Perform(IWeapon weapon)
        {
            weapon.AmmoHandler.SubtractAmmo(weapon.Ammo, BulletsToPerformShot);
        }
    }
}
