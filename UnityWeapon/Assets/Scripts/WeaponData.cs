using UnityEngine;


namespace Weapons.Data
{
    [System.Serializable]
    public class WeaponAction
    {
        public AudioSource Audio;
        public ParticleSystem Particle;
    }

    public enum WeaponState { None, Idle, Shooting, Reload }
    public enum WeaponType { None, Light, Medium, Heavy }



    public abstract class Range<T>
    {
        public T Value;
        public T Min;
        public T Max;


        public abstract float GetAverage();
    }

    public class FloatRange: Range<float>
    {
        public override float GetAverage()
        {
            float divisor = (Max - Min);

            return (divisor == 0.0f)? 0.0f : Value / divisor; 
        }
    }
}
