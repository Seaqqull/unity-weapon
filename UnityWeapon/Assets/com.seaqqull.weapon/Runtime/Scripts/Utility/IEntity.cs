using UnityEngine;

namespace Weapon.Utility
{
    public interface IEntity
    {
        int Id { get; }


        void ModifyHealth(int amount);
        void ApplyForce(Vector3 amount);
    }
}