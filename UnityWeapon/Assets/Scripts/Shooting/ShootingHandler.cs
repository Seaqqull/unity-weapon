using UnityEngine;


namespace Weapons.Shooting
{
    /// <summary>
    /// Used for weapon-specific checks
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Shooting/Handler")]
    public class ShootingHandler : global::Weapon.Utility.ComputingHandler
    {
        public virtual bool IsExecutable(Weapon weapon)
        {
            return true;
        }
    }
}
