using Weapons.Bullets;
using UnityEngine;


namespace Weapons.Ammo
{
    [System.Serializable]
    public class AmmoController : IAmmoController
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField] [HideInInspector] private string name = "Magazine";
#pragma warning restore CS0414
#endif
        [field: SerializeField] public BulletDataSO Bullet { get; private set; }

        [field: SerializeField] public int Capacity { get; private set; }
        [field: SerializeField] public int Amount { get; set; }
        [field: SerializeField] public int MagazineCapacity { get; private set; }
        [field: Space]
        [field: SerializeField] public bool IsAmountUnlimited { get; private set; }
        [field: SerializeField] public bool IsMagazineAmountUnlimited { get; private set; }
        [field: Space]
        [field: SerializeField] public float ReloadTime { get; private set; }

        public IBulletData BulletData => Bullet;
        public int MagazineCount { get; set; }
        public int MagazineAmount { get; set; }
    }
}
