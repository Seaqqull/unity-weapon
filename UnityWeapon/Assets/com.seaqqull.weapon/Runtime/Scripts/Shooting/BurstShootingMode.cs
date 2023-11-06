using Weapons.Bullets.Data;
using UnityEngine;


namespace Weapons.Shooting
{
    [CreateAssetMenu(menuName = "Weapon/Shooting/Burst")]
    public class BurstShootingMode : ShootingMode
    {
        private void EmmitBullet(IGameObjectWeapon weapon)
        {
            for (var i = 0; i < BulletsToPerformShot; i++)
            {
                // Some shot action
                var bulletObject = weapon.NextBullet();
                var bullet = bulletObject.GetComponent<IBullet>();

                if (bullet == null)
                {
#if UNITY_EDITOR
                    Debug.LogError("There is no bullet component in bullet prefab.", bulletObject);
#endif
                    return;
                }

                bullet.Bake(weapon.Ammo.BulletData);
                if (weapon.Accuracy)
                    bullet.BakeFlowDirection(weapon.Accuracy.CreateDirection());
                else if (weapon.BulletFlow)
                    bullet.BakeFlowDirection(weapon.BulletFlow);
#if UNITY_EDITOR
                else
                    Debug.Log("Weapon don't have Accuracy or BulletFlow component attached.", weapon.GameObj);
#endif

                bullet.Launch();
            }
        }
        
        public override void Perform(IWeapon weapon)
        {
            weapon.AmmoHandler.SubtractAmmo(weapon.Ammo, 1);

            switch (weapon)
            {
                case IGameObjectWeapon gameObjectWeapon: 
                    EmmitBullet(gameObjectWeapon);
                    break;
            }
        }

        public override bool IsExecutable(IWeapon weapon)
        {
            return weapon.Ammo.IsMagazineAmountUnlimited || weapon.Ammo.MagazineAmount >= 1;
        }
    }
}
