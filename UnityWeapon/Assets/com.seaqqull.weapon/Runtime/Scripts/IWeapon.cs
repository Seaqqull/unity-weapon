using Weapons.Ammo;
using Weapons.Data;
using Weapons.Shooting;

namespace Weapons
{
    public interface IWeapon
    {
        bool IsActionExecutable { get; }
        bool IsReloadPossible { get; }
        bool IsMagazineEmpty { get; }
        bool IsShotPossible { get; }
        WeaponState State { get; }
        WeaponType Type { get; }
        string Name { get; }

        IShootingHandler ShootingHandler { get; }
        IAmmoHandler AmmoHandler { get; }
        IAmmoController Ammo { get; }
        IShootingMode Mode { get; }

        void BreakAction();
        void ResumeAction();

        bool Shoot();
        bool Reload();
        void MakeShot();
        bool NextAmmoType();
        bool PreviousAmmoType();
        bool NextShootingMode();
        bool PreviousShootingMode();
        bool SetAmmoType(int index);
        bool SetShootingMode(int index);
    }
}