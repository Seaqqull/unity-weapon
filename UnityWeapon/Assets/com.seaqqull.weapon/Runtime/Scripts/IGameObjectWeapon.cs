using UnityEngine;

namespace Weapons
{
    public interface IGameObjectWeapon : IWeapon
    {
        Aiming.Accuracy Accuracy { get; }
        Transform BulletFlow { get; }
        GameObject GameObj { get; }

        GameObject NextBullet();
    }
}