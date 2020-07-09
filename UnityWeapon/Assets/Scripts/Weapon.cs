using System.Collections;
using UnityEngine;
using System;


namespace Weapons
{
    public class Weapon : BaseMonoBehaviour, Utility.Data.IRunLater
    {
        #region InnerClasses
        [Flags]
        public enum UIUpdateMode { None, MagazineAmount, OverallAmount, Progress = 4}
        [System.Serializable]
        public class AmmoComputeData : Utility.ComputingData<Ammo.AmmoHandler, Ammo.AmmoController> { }
        [System.Serializable]
        public class ShootingComputeObject : Utility.ComputingData<Shooting.ShootingHandler, Shooting.ShootingMode> { }
        #endregion

        #region EditorVariables
        [SerializeField] private Data.WeaponType _type;

        [SerializeField] protected int _flowLengthDraw;
        [SerializeField] protected Transform _bulletFlowPath;

        [SerializeField] protected int _activeAmmo;
        [SerializeField] protected AmmoComputeData _ammo;

        [SerializeField] protected int _activeShooting;
        [SerializeField] protected ShootingComputeObject _shooting;
        #endregion

        #region Variables
        protected Coroutine _progressCoroutine;
        protected Coroutine _actionCoroutine;
        
        protected Action<Weapon> _onMagazineAmmoChange;
        protected Action<float> _onActionProgress;
        protected Action<Weapon> _onAmmoChange;

        protected Data.FloatRange _actionProgress;
        protected Data.WeaponState _state;
        #endregion

        #region Fields
        public event Action<Weapon> OnMagazineAmmoChange
        {
            add { _onMagazineAmmoChange += value; }
            remove { _onMagazineAmmoChange -= value; }
        }
        public event Action<float> OnActionProgress
        {
            add { _onActionProgress += value; }
            remove { _onActionProgress -= value; }
        }
        public event Action<Weapon> OnAmmoChange
        {
            add { _onAmmoChange += value; }
            remove { _onAmmoChange -= value; }
        }

        public Ammo.AmmoHandler AmmoHandler
        {
            get { return _ammo.Handler; }
        }
        public Shooting.ShootingMode Mode
        {
            get { return _shooting.Data[_activeShooting]; }
        }
        public Ammo.AmmoController Ammo
        {
            get { return _ammo.Data[_activeAmmo]; }
        }
        public bool IsActionExecutable
        {
            get { return (_actionCoroutine == null); }
        }
        public Data.WeaponState State
        {
            get { return _state; }
        }
        public Data.WeaponType Type
        {
            get { return _type; }
        }
        public bool IsMagazineEmpty
        {
            get { return (Ammo.MagazineAmount == 0); }
        }
        public Transform BulletFlow
        {
            get { return _bulletFlowPath; }
        }
        #endregion


        protected override void Awake()
        {
            base.Awake();

            _actionProgress = new Data.FloatRange();

            ReloadInstant();
        }

        protected void OnDrawGizmos()
        {
            if (_bulletFlowPath == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_bulletFlowPath.position, _bulletFlowPath.position + _bulletFlowPath.forward * _flowLengthDraw);
        }
        

        protected bool CanReload()
        {
            if ((_actionCoroutine != null) ||
                        !_ammo.Handler.IsReloadPossible(_ammo.Data[_activeAmmo]))
                return false;
            return true;
        }

        protected bool ReloadInstant()
        {
            if (!CanReload())
                return false;

            _ammo.Handler.Reload(_ammo.Data[_activeAmmo]);

            return true;
        }

        protected IEnumerator UpdateProgress(float min, float max)
        {
            _actionProgress.Min = min;
            _actionProgress.Max = max;
            _actionProgress.Value = min;
            
            while (_actionProgress.Value < _actionProgress.Max)
            {
                UpdateUI(UIUpdateMode.Progress);

                yield return null;
                _actionProgress.Value += Time.deltaTime;
            }            

            _actionProgress.Min = 0.0f;
            _actionProgress.Max = 0.0f;
            _actionProgress.Value = 0.0f;

            _onActionProgress.Invoke(1.0f);
        }


        public bool Shoot()
        {
            if (!IsShotPossible()) return false;

            int currentSMode = _activeShooting;

            _shooting.Data[_activeShooting].Perform(this);
            
            _state = Data.WeaponState.Shooting;
            _progressCoroutine = 
                StartCoroutine(UpdateProgress(0, _shooting.Data[currentSMode].TimeBetweenShot));

            UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

            _actionCoroutine = RunLaterValued(() =>
            {
                _actionCoroutine = null;
                _state = Data.WeaponState.Idle;
            }, _shooting.Data[currentSMode].TimeBetweenShot);


            return true;
        }

        public bool Reload()
        {
            if (!CanReload()) return false;

            int currentAmmo = _activeAmmo;

            _state = Data.WeaponState.Reload;
            _progressCoroutine =
                StartCoroutine(UpdateProgress(0, _ammo.Data[currentAmmo].ReloadTime));

            _actionCoroutine = RunLaterValued(() =>
            {
                _ammo.Handler.Reload(_ammo.Data[_activeAmmo]);
                UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

                _actionCoroutine = null;
                _state = Data.WeaponState.Idle;
            }, _ammo.Data[currentAmmo].ReloadTime);


            return true;
        }

        public void UpdateUI(UIUpdateMode uiUpdateMode)
        {
            if ((uiUpdateMode & UIUpdateMode.OverallAmount) != 0)
                _onAmmoChange?.Invoke(this);
            if ((uiUpdateMode & UIUpdateMode.MagazineAmount) != 0)
                _onMagazineAmmoChange?.Invoke(this);
            if ((uiUpdateMode & UIUpdateMode.Progress) != 0)
                _onActionProgress?.Invoke(_actionProgress.GetAverage());
        }

        public void BreakAction()
        {
            if (_progressCoroutine != null)
            {
                StopCoroutine(_progressCoroutine);
                _progressCoroutine = null;
            }

            if (_actionCoroutine != null)
            {
                StopCoroutine(_actionCoroutine);
                _actionCoroutine = null;
            }                                        
        }

        public bool IsShotPossible()
        {
            if ((_actionCoroutine != null) ||
                !_shooting.Handler.IsExecutable(this) ||
                !_shooting.Data[_activeShooting].IsExecutable(this))
                return false;
            return true;
        }


        #region RunLater
        public void RunLater(Action method, float waitSeconds)
        {
            RunLaterValued(method, waitSeconds);
        }

        public Coroutine RunLaterValued(Action method, float waitSeconds)
        {
            if ((waitSeconds < 0) || (method == null))
                return null;

            return StartCoroutine(RunLaterCoroutine(method, waitSeconds));
        }

        public IEnumerator RunLaterCoroutine(Action method, float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            method();
        }
        #endregion
    }
}
