using UnityEngine;

[CreateAssetMenu(menuName = "Computing/Shooting/Handler")]
public class ShootingHandler : Utility.ComputingHandler
{
    // Specific weapon shooting checking
    public virtual bool IsExecutable(Weapon weapon)
    {
        return true;
    }
}
