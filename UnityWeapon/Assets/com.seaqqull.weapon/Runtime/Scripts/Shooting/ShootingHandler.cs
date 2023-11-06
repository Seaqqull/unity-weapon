using Weapon.Computing;
using UnityEngine;


namespace Weapons.Shooting
{
    /// <summary>
    /// Used for weapon-specific checks
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Shooting/Handler")]
    public class ShootingHandler : ComputingHandler, IShootingHandler
    {
        public virtual bool IsExecutable(IWeapon weapon)
        {
            return true;
        }
    }
}
