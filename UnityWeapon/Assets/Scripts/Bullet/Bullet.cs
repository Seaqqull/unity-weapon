using UnityEngine;


namespace Weapons.Bullets
{
    [CreateAssetMenu(menuName = "Bullet/Create")]
    public class Bullet : ScriptableObject
    {
        public GameObject BulletObject;
        public bool LookRotation = true;

        public int Damage;
        public float Speed;
        public float Range;
        public LayerMask TargetMask;
    }
}
