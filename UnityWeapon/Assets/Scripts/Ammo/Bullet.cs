using UnityEngine;
using Attribute;

[CreateAssetMenu(menuName = "Bullet/Create")]
public class Bullet : ScriptableObject
{
    [SerializeField] protected GameObject _bullet;
    [SerializeField] protected WeaponAction _shotAction;
    [SerializeField] protected WeaponAction _flyAction;
    [SerializeField] protected WeaponAction _collisionAction;
    [SerializeField] protected StaticAttributes _attributes;
    [SerializeField] [TagSelectorAttribute] protected string[] _enemyTags;
    [SerializeField] [TagSelectorAttribute] protected string[] _collisionTags;    
}
