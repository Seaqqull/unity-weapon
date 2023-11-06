using Weapons.Bullets;


namespace Weapons.Ammo
{
    public interface IAmmoController
    {
        IBulletData BulletData { get; }
        
        bool IsMagazineAmountUnlimited { get; }
        bool IsAmountUnlimited { get; }
        int MagazineCapacity { get; }
        float ReloadTime { get; }
        int Capacity { get; }

        int MagazineAmount { get; set; }
        int MagazineCount { get; set; }
        int Amount { get; set; }
    }
}