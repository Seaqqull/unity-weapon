using UnityEngine;


namespace Weapons.Shooting
{
    /// <summary>
    /// Used for weapon-specific checks
    /// </summary>
    [CreateAssetMenu(menuName = "Weapon/Shooting/Handler")]
    public class ShootingHandler : Utility.ComputingHandler
    {
        public virtual bool IsExecutable(Weapons.Weapon weapon)
        {
            return true;
        }
    }
}
