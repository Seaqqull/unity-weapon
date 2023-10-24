


namespace Weapons.Shooting
{
    public abstract class ShootingMode : global::Weapon.Utility.ComputingHandler
    {
        public int BulletsToPerformShot = 1;
        public float TimeBetweenShot = 0.1f;


        public abstract bool IsExecutable(Weapon weapon);


        public virtual void Perform(Weapon weapon)
        {
            weapon.AmmoHandler.SubtractAmmo(weapon.Ammo, BulletsToPerformShot);
        }
    }
}
