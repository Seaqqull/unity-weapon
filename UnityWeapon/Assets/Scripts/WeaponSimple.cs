using System.Collections;
using Utility.Data;
using UnityEngine;
using System;


namespace Weapons
{
    public class WeaponSimple : MonoBehaviour//, IRunLater, IAttributeContainer
    {
        //[System.Serializable]
        //public class AmmoComputeData : Utility.ComputingData<AmmoHandler, AmmoData, AmmoData.Ammo> { }
        //[System.Serializable]
        //public class ShootingComputeObject : Utility.ComputingObject<ShootingHandler, ShootingMode> { }
        //[System.Serializable]
        //public struct Bullet
        //{
        //    public Transform Parent;
        //    public Transform Start;
        //    public Vector3 Direction;
        //}


        //[SerializeField] protected WeaponAction _shotAction;
        //[SerializeField] protected WeaponAction _reloadAction;
        //[SerializeField] protected WeaponAction _changeAmmoAction;
        //[SerializeField] protected WeaponAction _changeShootingModeAction;

        //[SerializeField] protected Bullet _bulletCharacteristics;

        //[SerializeField] protected int _activeAmmo;
        //[SerializeField] protected AmmoComputeData _ammo;
        //[SerializeField] protected int _activeShooting;
        //[SerializeField] protected ShootingComputeObject _shooting;
        ////[SerializeField] protected AttributeModifier[] _attributes;
        //[SerializeField] protected StaticAttributes _attributeStatic;
        //[SerializeField] protected StringReference _reloadAttribute;
        //[SerializeField] protected StringReference _shootingAttribute;

        //protected Coroutine _actionCoroutine;


        //private void Awake()
        //{
        //    _attributeStatic.BakeAttributes();
        //    //the same for dynamic
        //    _ammo.BakeData();

        //    Debug.Log("Begin of coroutation");
        //    _actionCoroutine = RunLaterValued(() => {
        //        _actionCoroutine = null;
        //        Debug.Log("End of coroutation");
        //    }, 3.0f);
        //}

        //public bool Shoot()
        //{
        //    if (_actionCoroutine != null)
        //        return false;

        //    bool flag = _shooting.Handler.IsExecutable(this);
        //    if (!flag) return false;

        //    //_actionCoroutine = RunLaterValued(() => {
        //    //    _actionCoroutine = null;
        //    //    _ammo.Handler.Reload(_ammo.Data[_activeAmmo]);
        //    //}, _attributeManager.GetValue(_reloadAttribute));

        //    return true;
        //}
        //public bool Reload()
        //{
        //    if (_actionCoroutine != null)
        //        return false;

        //    bool flag = _ammo.Handler.IsReloadPossible(_ammo.Data[_activeAmmo]);
        //    if (!flag) return false;

        //    //_actionCoroutine = RunLaterValued(() => {
        //    //    _actionCoroutine = null;
        //    //    _ammo.Handler.Reload( _ammo.Data[_activeAmmo]);
        //    //}, _attributeManager.GetValue(_reloadAttribute));

        //    return true;
        //}
        //public bool PrevAmmoMode() { return false; }
        //public bool NextAmmoMode() { return false; }
        //public bool PrevShootingMode() { return false; }
        //public bool NextShootingMode() { return false; }


        //public void BreakAction()
        //{        
        //    StopCoroutine(_actionCoroutine);
        //    _actionCoroutine = null;
        //}

        //public bool IsActionExecutable()
        //{
        //    return (_actionCoroutine == null);
        //}


        //public float GetAttributeAbsolute(string key)
        //{
        //    _attributeStatic.Absolutes.TryGetValue(key, out float value);
        //    return value;
        //}

        //public float GetAttributeRelative(string key)
        //{
        //    if (_attributeStatic.Relatives.TryGetValue(key, out float value))
        //        return value;
        //    return 1.0f;
        //}


        //public void RunLater(Action method, float waitSeconds)
        //{
        //    RunLaterValued(method, waitSeconds);
        //}

        //public Coroutine RunLaterValued(Action method, float waitSeconds)
        //{
        //    if ((waitSeconds < 0) || (method == null))
        //        return null;

        //    return StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        //}

        //public IEnumerator RunLaterCoroutine(Action method, float waitSeconds)
        //{
        //    yield return new WaitForSeconds(waitSeconds);
        //    method();
        //}

    }
}
