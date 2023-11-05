using Weapons.Bullets;

namespace Weapons.Ammo
{
    public interface IAmmoController
    {
        IBulletData BulletData { get; }

        int Capacity { get; }
        int Amount { get; set; }
        int MagazineCapacity { get; }
        
        bool IsAmountUnlimited { get; }
        bool IsMagazineAmountUnlimited { get; }
        
        float ReloadTime { get; }

        int MagazineCount { get; set; }
        int MagazineAmount { get; set; }
    }
}