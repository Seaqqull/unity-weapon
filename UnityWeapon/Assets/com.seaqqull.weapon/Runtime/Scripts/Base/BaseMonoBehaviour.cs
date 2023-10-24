using UnityEngine;

namespace Weapon.Base
{
    public class BaseMonoBehaviour : MonoBehaviour
    {
        public GameObject GameObj { get; private set; }
        public Transform Transform { get; private set; }
        public Vector3 Position => Transform.position;
        public Vector3 Forward => Transform.forward;

        public Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }


        protected virtual void Awake()
        {
            Transform = transform;
            GameObj = gameObject;
        }
    }
}