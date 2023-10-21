using UnityEngine;
using System;


namespace Weapons.Data
{
    // [Serializable]
    // public class WeaponAction
    // {
    //     public AudioSource Audio;
    //     public ParticleSystem Particle;
    // }

    // [Serializable]
    // public class ActionResponse
    // {
    //     public UnityEvent UiResponse;
    //     public Action CodeResponse;
    // }

    public enum UiUpdateRate { FPS_Umlimited = 0, FPS_30 = 30, FPS_60 = 60, FPS_120 = 120 }
    
    public enum WeaponState { None, Idle, Shooting, Reload }
    public enum WeaponType { None, Light, Medium, Heavy }


    public abstract class Range<T>
    {
        public abstract float Progress { get; }

        public T Value;
        public T Min;
        public T Max;

        public void Reset()
        {
            Value = default;
            Max = default;
            Min = default;
        }

        public void Update(T value, T min, T max)
        {
            Value = value;
            Max = max;
            Min = min;
        }
    }

    public class FloatRange : Range<float>
    {
        public override float Progress => Mathf.InverseLerp(Min, Max, Value); 
    }

    public class StateInfo
    {        
        public float TimeRemaining { get; private set; }
        public float TimeBegin { get; private set; }
        public float Period { get; private set; }
        public bool IsEmpty { get; private set; }
        public int Id { get; private set; }


        public void Forget()
        {
            IsEmpty = true;

            TimeRemaining = default;
            TimeBegin = default;
            Id = default;
        }

        public void CalculateRemaining()
        {
            TimeRemaining = Math.Max(Period - (Time.time - TimeBegin), 0.0f);
        }

        public void Remember(int id, float period)
        {
            IsEmpty = false;

            TimeRemaining = period;
            TimeBegin = Time.time;
            Period = period;
            Id = id;
        }
    }
}
