using UnityEngine;
using Weapon.Computing;


namespace Weapons.Shooting
{
    public abstract class ShootingMode : ComputingHandler, IShootingMode
    {
        [field: SerializeField] public virtual int BulletsToPerformShot { get; private set; } = 1;
        [field: SerializeField] public virtual float TimeBetweenShot { get; private set; } = 0.1f;


        public abstract bool IsExecutable(IWeapon weapon);


        public virtual void Perform(IWeapon weapon)
        {
            weapon.AmmoHandler.SubtractAmmo(weapon.Ammo, BulletsToPerformShot);
        }
    }
}