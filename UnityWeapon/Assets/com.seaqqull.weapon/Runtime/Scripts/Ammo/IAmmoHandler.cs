namespace Weapons.Ammo
{
    public interface IAmmoHandler
    {
        void Reload(IAmmoController data);
        bool IsReloadPossible(IAmmoController data);
        int AddAmmo(IAmmoController data, int count);
        void SubtractAmmo(IAmmoController data, int shootCount);
        bool HasRequiredAmount(IAmmoController data, int ammoAmount);
    }
}