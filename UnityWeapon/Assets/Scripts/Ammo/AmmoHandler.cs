using UnityEngine;
using static AmmoData;

[CreateAssetMenu(menuName = "Computing/Ammo/Handler")]
public class AmmoHandler : Utility.ComputingHandler
{
    public void Reload(Ammo data)
    {
        data.MagazineCount = data.Capacity / data.MagazineCapacity;
        data.MagazineAmount = data.Capacity % data.MagazineCapacity;

        if ((data.MagazineAmount != data.MagazineCapacity) &&
            (data.MagazineCount > 0))
        {
            data.MagazineAmount = data.MagazineCapacity;
            data.MagazineCount--;
        }
    }

    public bool IsReloadPossible(Ammo data)
    {
        return data.Amount > 0;
    }

    public void AddAmmo(Ammo data, int count)
    {
        if (count > data.Capacity - data.Amount)
            data.Amount = data.Capacity;
        else
            data.Amount += count;
    }

    public void SubtractAmmo(Ammo data, int shootCount)
    {
        if ((data.IsUnlimited) ||
            (data.MagazineAmount == 0)) return;

        data.MagazineAmount -= shootCount;
        data.Amount -= shootCount;
    }

    public bool HasRequiredAmount(Ammo data, int ammoAmount)
    {
        return (data.IsUnlimited) ? true :
               (ammoAmount > data.MagazineAmount) ? false : true;
    }
}
