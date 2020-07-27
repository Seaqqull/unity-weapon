using UnityEngine.Events;
using UnityEngine;
using System;


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


    [System.Serializable]
    public class ActionResponce
    {
        public UnityEvent UiResponce;
        public System.Action CodeResponce;
    }

    public class StateInfo
    {        
        public float TimeRemaining { get; private set; }
        public float TimeBegin { get; private set; }
        public float Period { get; private set; }
        public int Id { get; private set; }

        public bool IsEmpty => TimeBegin.Equals(float.NaN);


        public void Forget()
        {
            TimeBegin = float.NaN;
            TimeRemaining = float.NaN;
        }

        public void CalculateRemaining()
        {
            TimeRemaining = Math.Max(Period - (Time.time - TimeBegin), 0.0f);
        }

        public void Remember(int id, float period)
        {
            TimeBegin = Time.time;
            Period = period;
            Id = id;
        }
    }
}
