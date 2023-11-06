using UnityEngine;
using System;


namespace Weapon.Base
{
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
        
        public void Remember(int id, float period)
        {
            IsEmpty = false;

            TimeRemaining = period;
            TimeBegin = Time.time;
            Period = period;
            Id = id;
        }

        public void CalculateRemaining(float progress)
        {
            TimeRemaining = Math.Max(Period - progress, 0.0f);
        }
    }
}