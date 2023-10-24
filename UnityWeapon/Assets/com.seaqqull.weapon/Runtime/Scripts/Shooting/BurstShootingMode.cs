using UnityEngine;
using Weapons.Bullets.Data;


namespace Weapons.Shooting
{
    [CreateAssetMenu(menuName = "Weapon/Shooting/Burst")]
    public class BurstShootingMode : ShootingMode
    {
        public override void Perform(Weapon weapon)
        {
            weapon.AmmoHandler.SubtractAmmo(weapon.Ammo, 1);

            for (var i = 0; i < BulletsToPerformShot; i++)
            {
                // Some shot action
                var bulletObject = weapon.NextBullet();
                var bullet = bulletObject.GetComponent<IBullet>();

                if (bullet == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("There is no bullet component in bullet prefab.", weapon.Ammo.Bullet.BulletObject);
#endif
                    return;
                }

                bullet.Bake(weapon.Ammo.Bullet);
                if (weapon.Accuracy)
                    bullet.BakeFlowDirection(weapon.Accuracy.GetDirectionVector());
                else if (weapon.BulletFlow)
                    bullet.BakeFlowDirection(weapon.BulletFlow);
#if UNITY_EDITOR
                else
                    Debug.Log("Weapon don't have Accuracy or BulletFlow component attached.", weapon);
#endif

                bullet.Launch();
            }
        }

        public override bool IsExecutable(Weapon weapon)
        {
            return weapon.Ammo.IsMagazineAmountUnlimited || weapon.Ammo.MagazineAmount >= 1;
        }
    }
}
