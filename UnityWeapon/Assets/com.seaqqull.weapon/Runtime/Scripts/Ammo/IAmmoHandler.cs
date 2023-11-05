namespace Weapons.Ammo
{
    public interface IAmmoHandler
    {
        public void Reload(IAmmoController data);
        public bool IsReloadPossible(IAmmoController data);
        public int AddAmmo(IAmmoController data, int count);
        public void SubtractAmmo(IAmmoController data, int shootCount);
        public bool HasRequiredAmount(IAmmoController data, int ammoAmount);
    }
}