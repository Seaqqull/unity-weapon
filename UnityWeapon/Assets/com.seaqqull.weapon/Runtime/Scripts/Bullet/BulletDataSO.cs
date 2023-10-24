using UnityEngine;


namespace Weapons.Bullets
{
    [CreateAssetMenu(menuName = "Bullet/Create")]
    public class BulletDataSO : ScriptableObject
    {
        [field: SerializeField] public GameObject BulletObject { get; private set; }
        [field: SerializeField] public bool LookRotation { get; private set; } = true;
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public LayerMask TargetMask { get; private set; }
    }
}
