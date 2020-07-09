using UnityEngine;


namespace Weapons.Ammo
{
    [System.Serializable]
    public class AmmoController
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField] [HideInInspector] private string Name = "Magazine";
#pragma warning restore CS0414
#endif
        public Bullets.Bullet Bullet;

        public int Capacity;
        public int Amount;
        public int MagazineCapacity;

        public bool IsAmountUnlimited;
        public bool IsMagazineAmountUnlimited;

        public float ReloadTime;

        [HideInInspector] public int MagazineCount;
        [HideInInspector] public int MagazineAmount;
    }
}
