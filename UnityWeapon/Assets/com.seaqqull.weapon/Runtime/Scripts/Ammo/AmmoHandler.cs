﻿using Weapon.Computing;
using UnityEngine;


namespace Weapons.Ammo
{
    [CreateAssetMenu(menuName = "Weapon/Ammo/Handler")]
    public class AmmoHandler : ComputingHandler, IAmmoHandler
    {
        public void Reload(IAmmoController data)
        {
            data.MagazineCount = data.Amount / data.MagazineCapacity;
            data.MagazineAmount = data.Amount % data.MagazineCapacity;
            if (data.MagazineAmount == data.MagazineCapacity || data.MagazineCount <= 0) return;

            data.MagazineAmount = data.MagazineCapacity;
            data.MagazineCount--;
        }

        public bool IsReloadPossible(IAmmoController data)
        {
            return data.Amount > 0 && data.MagazineAmount < data.MagazineCapacity;
        }

        public int AddAmmo(IAmmoController data, int count)
        {
            if (count > data.Capacity - data.Amount)
                count = data.Capacity - data.Amount;

            data.Amount += count;
            return count;
        }

        public void SubtractAmmo(IAmmoController data, int shootCount)
        {
            if (data.MagazineAmount == 0 || (data.IsAmountUnlimited && data.IsMagazineAmountUnlimited)) return;

            data.MagazineAmount -= shootCount;
            if (!data.IsAmountUnlimited)
                data.Amount -= shootCount;
        }

        public bool HasRequiredAmount(IAmmoController data, int ammoAmount)
        {
            return data.IsMagazineAmountUnlimited || !(ammoAmount > data.MagazineAmount);
        }
    }
}
