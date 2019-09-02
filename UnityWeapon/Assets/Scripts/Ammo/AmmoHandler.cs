using UnityEngine;

[CreateAssetMenu(menuName = "Computing/Ammo/Handler")]
public class AmmoHandler : Utility.ComputingHandler
{
    public void Reload(AmmoData data)
    {
        data.Data.MagazineCount = data.Data.Capacity / data.Data.MagazineCapacity;
        data.Data.MagazineAmount = data.Data.Capacity % data.Data.MagazineCapacity;

        if ((data.Data.MagazineAmount != data.Data.MagazineCapacity) &&
            (data.Data.MagazineCount > 0))
        {
            data.Data.MagazineAmount = data.Data.MagazineCapacity;
            data.Data.MagazineCount--;
        }
    }

    public bool IsReloadPossible(AmmoData data)
    {
        return data.Data.Amount > 0;
    }

    public void AddAmmo(AmmoData data, int count)
    {
        if (count > data.Data.Capacity - data.Data.Amount)
            data.Data.Amount = data.Data.Capacity;
        else
            data.Data.Amount += count;
    }

    public void SubtractAmmo(AmmoData data, int shootCount)
    {
        if ((data.Data.IsUnlimited) ||
            (data.Data.MagazineAmount == 0)) return;

        data.Data.MagazineAmount -= shootCount;
        data.Data.Amount -= shootCount;
    }

    public bool HasRequiredAmount(AmmoData data, int ammoAmount)
    {
        return (data.Data.IsUnlimited) ? true :
               (ammoAmount > data.Data.MagazineAmount) ? false : true;
    }
}
