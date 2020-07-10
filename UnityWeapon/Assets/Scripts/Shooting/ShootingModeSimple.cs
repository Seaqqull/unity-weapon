using UnityEngine;


namespace Weapons.Shooting
{
    [CreateAssetMenu(menuName = "Weapon/Shooting/Single")]
    public class SimpleShootingMode : ShootingMode
    {
        public override void Perform(Weapon weapon)
        {
            base.Perform(weapon);

            for (int i = 0; i < BulletsToPerformShot; i++)
            {
                GameObject bullet = Instantiate(weapon.Ammo.Bullet.BulletObject);
                Bullets.ActiveBullet aBullet = bullet.GetComponent<Bullets.ActiveBullet>();

    #if UNITY_EDITOR
                if (aBullet == null)
                {
                    Debug.LogError("There is no bullet component in bullet prefab.", weapon.Ammo.Bullet.BulletObject);
                    return;
                }
    #endif

                aBullet.BakeData(weapon.Ammo.Bullet);

                if (weapon.Accuracy)
                    aBullet.BakeFlowDirection(weapon.Accuracy.GetDirectedVector());
                else if (weapon.BulletFlow)
                    aBullet.BakeFlowDirection(weapon.BulletFlow);
#if UNITY_EDITOR
                else
                    Debug.Log("Weapon don't have Accuracy or BulletFlow component attached.", weapon);
#endif
                aBullet.Lunch();
            }
        }

        public override bool IsExecutable(Weapon weapon)
        {
            return (weapon.Ammo.IsMagazineAmountUnlimited ||
                   (weapon.Ammo.MagazineAmount >= BulletsToPerformShot));
        }

    }
}
