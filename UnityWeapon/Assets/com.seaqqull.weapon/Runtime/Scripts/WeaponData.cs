﻿using UnityEngine;
using System;
using Weapon.Base;


namespace Weapons.Data
{
    public enum ChangeDirection { Back, Forward }
    public enum WeaponType { None, Light, Medium, Heavy }
    public enum WeaponState { None, Idle, Shooting, Reload }
    [Flags] public enum UIUpdateMode { None, MagazineAmount, OverallAmount, Progress = 4 }
    public enum UiUpdateRate { FPS_Umlimited = 0, FPS_30 = 30, FPS_60 = 60, FPS_120 = 120 }

    public class FloatRange : Range<float>
    {
        public override float Progress => Mathf.InverseLerp(Min, Max, Value); 
    }
    [Serializable] public class AmmoComputeData : global::Weapon.Utility.ComputingData<Ammo.AmmoHandler, Ammo.AmmoController> { }
    [Serializable] public class ShootingComputeData : global::Weapon.Utility.ComputingData<Shooting.ShootingHandler, Shooting.ShootingMode> { }
}