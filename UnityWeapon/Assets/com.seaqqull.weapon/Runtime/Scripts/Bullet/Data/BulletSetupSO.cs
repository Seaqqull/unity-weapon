using Weapons.Aiming.Following;
using UnityEngine;


namespace Weapons.Bullets
{
    [CreateAssetMenu(menuName = "Weapon/Bullets/Create")]
    public class BulletSetupSO : ScriptableObject, IBulletSetup, IBulletData, IFollowerFactoryProvider
    {
        [field: SerializeField] public GameObject BulletObject { get; private set; }
        [field: SerializeField] public bool LookRotation { get; private set; } = true;
        [SerializeField] private FollowerFactorySO _follower;
        [field: Space]
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Range { get; private set; }
        [field: Space]
        [field: SerializeField] public LayerMask TargetMask { get; private set; }

        public IFollowerFabric FollowerFactory => _follower;
        public IFollowerFactoryProvider Follow => this;
        public IBulletData Data => this;
    }
}