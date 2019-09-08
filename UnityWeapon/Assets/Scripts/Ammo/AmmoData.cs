using UnityEngine;
using Attribute;

[CreateAssetMenu(menuName = "Computing/Ammo/Data")]
public class AmmoData : Utility.ComputingData<AmmoData.Ammo>
{
    [System.Serializable]
    public struct Ammo
    {
        [SerializeField] public Bullet Bullet;
        [SerializeField] public int Capacity;
        [SerializeField] public int Amount;
        [SerializeField] public int MagazineCapacity;
        [SerializeField] public bool IsUnlimited;
        [SerializeField] public StaticAttributes Attributes;

        [HideInInspector] public int MagazineCount;
        [HideInInspector] public int MagazineAmount;
    }
}
