using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bullet/Create")]
public class Bullet : ScriptableObject
{
    [SerializeField] protected GameObject _bullet;
    [SerializeField] protected WeaponAction _shotAction;
    [SerializeField] protected WeaponAction _flyAction;
    [SerializeField] protected WeaponAction _collisionAction;
    [SerializeField] protected AttributeModifier[] _attributes;
    [SerializeField] [TagSelectorAttribute] protected string[] _enemyTags;
    [SerializeField] [TagSelectorAttribute] protected string[] _collisionTags;    
}
