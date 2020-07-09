using UnityEngine;


namespace Weapons.Ammo
{
    [CreateAssetMenu(menuName = "Weapon/Ammo/Handler")]
    public class AmmoHandler : Utility.ComputingHandler
    {
        public virtual void Reload(AmmoController data)
        {
            data.MagazineCount = data.Amount / data.MagazineCapacity;
            data.MagazineAmount = data.Amount % data.MagazineCapacity;

            if ((data.MagazineAmount != data.MagazineCapacity) &&
                (data.MagazineCount > 0))
            {
                data.MagazineAmount = data.MagazineCapacity;
                data.MagazineCount--;
            }
        }

        public virtual bool IsReloadPossible(AmmoController data)
        {
            return data.Amount > 0;
        }

        public virtual int AddAmmo(AmmoController data, int count)
        {
            if (count > data.Capacity - data.Amount)
                count = data.Capacity - data.Amount;

            data.Amount += count;
            return count;
        }

        public virtual void SubtractAmmo(AmmoController data, int shootCount)
        {
            if ((data.MagazineAmount == 0) ||
                (data.IsAmountUnlimited && data.IsMagazineAmountUnlimited)) return;

            if (!data.IsMagazineAmountUnlimited)
            {
                data.MagazineAmount -= shootCount;
                if (!data.IsAmountUnlimited)
                    data.Amount -= shootCount;
            }
        }

        public virtual bool HasRequiredAmount(AmmoController data, int ammoAmount)
        {
            return (data.IsMagazineAmountUnlimited) ? true :
                   (ammoAmount > data.MagazineAmount) ? false : true;
        }

    }
}
