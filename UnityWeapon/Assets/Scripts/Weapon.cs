using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Weapon.Storage;
using Weapon.Storage.Data;
using Weapons.Data;


namespace Weapons
{
    public class Weapon : BaseMonoBehaviour, global::Weapon.Utility.IRunLater
    {
        #region Constants
        private static readonly string BULLETS_CONTAINER = new("Bullets");
        private static readonly Color BULLET_FLOW_COLOR = Color.yellow;
        #endregion

        #region InnerClasses
        protected enum ChangeDirection { Back, Forward }
        [Flags] protected enum UIUpdateMode { None, MagazineAmount, OverallAmount, Progress = 4 }
        [Serializable] public class AmmoComputeData : global::Weapon.Utility.ComputingData<Ammo.AmmoHandler, Ammo.AmmoController> { }
        [Serializable] public class ShootingComputeData : global::Weapon.Utility.ComputingData<Shooting.ShootingHandler, Shooting.ShootingMode> { }
        #endregion

        #region EditorVariables
        [SerializeField] private Data.WeaponType _type;
        [Header("Initialization")] 
        [SerializeField] private bool _reloadOnStartup;
        [Space]
        [SerializeField] private int _poolAmount;
        [SerializeField] private int _expansionAmount;
        [SerializeField] private int _reductionAmount;
        [Header("Aiming")]
        [SerializeField] protected Aiming.Accuracy _accuracy;
        [SerializeField] protected Transform _bulletFlowPath;
        [SerializeField] protected int _flowLengthDraw;
        [Header("Events")] 
        [SerializeField] private UnityEvent _onShot;
        [SerializeField] private UnityEvent _onReloaded;
        [SerializeField] private UnityEvent _onAmmoChanged;
        [SerializeField] private UnityEvent _onShootingModeChanged;
        [Space] 
        [SerializeField] private Data.UiUpdateRate _uiUpdateRate = UiUpdateRate.FPS_Umlimited;
        [SerializeField] private UnityEvent<float> _onStateProgressUI;
        [SerializeField] private UnityEvent<Weapon> _onMagazineAmmoChangedUI;
        [SerializeField] private UnityEvent<Weapon> _onAmmoChangedUI;
        [Header("Ammo")]
        [SerializeField] protected int _activeAmmo;
        [SerializeField] protected AmmoComputeData _ammo;
        [Header("Shooting")]
        [SerializeField] protected int _activeShooting;
        [SerializeField] protected ShootingComputeData _shooting;
        #endregion

        #region Variables
        protected Data.FloatRange _actionProgress = new();
        protected Data.StateInfo _stateInfo = new();
        protected WaitForSeconds _uiUpdateTime;
        protected Coroutine _actionCoroutine;
        protected GameObject _bulletsStorage;
        protected Action _stateResultAction;
        protected Data.WeaponState _state;
        protected IStorage _storage;
        #endregion

        #region Fields
        public event UnityAction OnShootingModeChange
        {
            add => _onShootingModeChanged.AddListener(value);
            remove => _onShootingModeChanged.RemoveListener(value);
        }
        public event UnityAction OnAmmoChange
        {
            add => _onAmmoChanged.AddListener(value);
            remove => _onAmmoChanged.RemoveListener(value);
        }
        public event UnityAction OnReload
        {
            add => _onReloaded.AddListener(value);
            remove => _onReloaded.RemoveListener(value);
        }
        public event UnityAction OnShot
        {
            add => _onShot.AddListener(value);
            remove => _onShot.RemoveListener(value);
        }

        public event UnityAction<Weapon> OnMagazineAmmoChangedUI
        {
            add => _onMagazineAmmoChangedUI.AddListener(value);
            remove => _onMagazineAmmoChangedUI.RemoveListener(value);
        }
        public event UnityAction<float> OnStateProgressUI
        {
            add => _onStateProgressUI.AddListener(value);
            remove => _onStateProgressUI.RemoveListener(value);
        }
        public event UnityAction<Weapon> OnAmmoChangedUI
        {
            add => _onAmmoChangedUI.AddListener(value);
            remove => _onAmmoChangedUI.RemoveListener(value);
        }
        
        public Shooting.ShootingMode Mode => _shooting.Data[_activeShooting];
        public Ammo.AmmoController Ammo => _ammo.Data[_activeAmmo];
        public Ammo.AmmoHandler AmmoHandler => _ammo.Handler;
        public Transform BulletFlow => _bulletFlowPath;
        public Aiming.Accuracy Accuracy => _accuracy;
        public Data.WeaponState State => _state;
        public Data.WeaponType Type => _type;

        public bool IsActionExecutable => _actionCoroutine == null;
        public bool IsReloadPossible => 
            IsActionExecutable && 
            _ammo.Handler.IsReloadPossible(_ammo.Data[_activeAmmo]);
        public bool IsMagazineEmpty => (Ammo.MagazineAmount == 0);
        public bool IsShotPossible =>
            IsActionExecutable &&
            _shooting.Handler.IsExecutable(this) &&
            _shooting.Data[_activeShooting].IsExecutable(this);
        public Data.UiUpdateRate UiUpdateRate
        {
            get => _uiUpdateRate;
            set
            {
                _uiUpdateRate = value;
                if (_uiUpdateRate == UiUpdateRate.FPS_Umlimited)
                    _uiUpdateTime = null;
                else
                    _uiUpdateTime = new WaitForSeconds(1.0f / (int) UiUpdateRate);
            }
        }
        #endregion


        protected override void Awake()
        {
            base.Awake();

            _bulletsStorage = new GameObject(BULLETS_CONTAINER);
            _bulletsStorage.transform.parent = Transform;
            _storage = new Storage(_bulletsStorage);

            if (_reloadOnStartup)
                ReloadInstantly();

            for (var i = 0; i < _ammo.Data.Count; i++)
                _storage.Populate(
                    _ammo.Data[i].Bullet.BulletObject, 
                    new StorageData(i, _poolAmount, _expansionAmount, _reductionAmount));

            UiUpdateRate = _uiUpdateRate;
        }
        
        protected void OnEnable()
        {
            ResumeAction();
        }

        protected void OnDisable()
        {
            BreakAction();
        }

        protected void OnDrawGizmos()
        {
            if (_bulletFlowPath == null) return;

            Gizmos.color = BULLET_FLOW_COLOR;
            Gizmos.DrawLine(_bulletFlowPath.position, _bulletFlowPath.position + _bulletFlowPath.forward * _flowLengthDraw);
        }


        private static int GetFromDirection(ChangeDirection direction, int current, int step, int leftBound, int rightBound)
        {
            switch (direction)
            {
                case ChangeDirection.Back:
                    current -= step;
                    if (current < leftBound)
                        current = rightBound - (leftBound - current);
                    return current;
                case ChangeDirection.Forward:
                    current += step;
                    if (current > rightBound)
                        current = leftBound + (current - rightBound);
                    return current;
                default:
                    return current;
            }
        }


        protected bool ReloadInstantly()
        {
            if (!IsReloadPossible) return false;

            _ammo.Handler.Reload(_ammo.Data[_activeAmmo]);
            return true;
        }

        private bool ChangeAmmoType(int modeIndex)
        {
            if (!IsActionExecutable || modeIndex < 0 || modeIndex >= _ammo.Data.Count) return false;

            _activeAmmo = modeIndex;
            return true;
        }
        
        private bool ChangeShootingMode(int modeIndex)
        {
            if (!IsActionExecutable || modeIndex < 0 || modeIndex >= _shooting.Data.Count) return false;
            
            _activeShooting = modeIndex;
            return true;
        }
        
        protected void UpdateUI(UIUpdateMode uiUpdateMode)
        {
            if (uiUpdateMode.HasFlag(UIUpdateMode.OverallAmount))
                _onAmmoChangedUI?.Invoke(this);
            if (uiUpdateMode.HasFlag(UIUpdateMode.MagazineAmount))
                _onMagazineAmmoChangedUI?.Invoke(this);
            if (uiUpdateMode.HasFlag(UIUpdateMode.Progress))
                _onStateProgressUI.Invoke(_actionProgress.Progress);
        }

        private bool ChangeAmmoType(ChangeDirection direction)
        {
            if (!IsActionExecutable) return false;
            if (_ammo.Data.Count <= 1) return true;

            _activeAmmo = GetFromDirection(direction, _activeAmmo, 1, 0, _ammo.Data.Count - 1);
            return true;
        }

        protected IEnumerator ActionRoutine(float min, float max)
        {
            _actionProgress.Update(min, min, max);
            while (_actionProgress.Value < _actionProgress.Max)
            {
                UpdateUI(UIUpdateMode.Progress);
                var beginWaitTime = Time.time;

                yield return _uiUpdateTime;
                _actionProgress.Value += (Time.time - beginWaitTime);
            }

            _actionProgress.Value = _actionProgress.Max;
            UpdateUI(UIUpdateMode.Progress);
            _actionProgress.Reset();

            _stateResultAction?.Invoke();
            _stateResultAction = null;
            _actionCoroutine = null;
        }
        
        private bool ChangeShootingMode(ChangeDirection direction)
        {
            if (!IsActionExecutable) return false; 
            if (_shooting.Data.Count <= 1) return true;

            _activeShooting = GetFromDirection(direction, _activeShooting, 1, 0, _shooting.Data.Count - 1);
            return true;
        }

        private void Do(Action action, Data.WeaponState state, float period)
        {
            _state = state;
            _stateInfo.Remember((int)state, period);

            _actionCoroutine = StartCoroutine(ActionRoutine(0, period));
            _stateResultAction = action;
        }

        public GameObject NextBullet()
        {
            return _storage.Pool(_activeAmmo);
        }

        public void MakeShot()
        {
            Shoot();
        }

        public bool Shoot()
        {
            if (!IsShotPossible) return false;

            _shooting.Data[_activeShooting].Perform(this);

            _onShot.Invoke();

            Do(() =>
            {
                _state = Data.WeaponState.Idle;
                _stateInfo.Forget();
            }, Data.WeaponState.Shooting, _shooting.Data[_activeShooting].TimeBetweenShot);
            UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

            return true;
        }

        public bool Reload()
        {
            if (!IsReloadPossible) return false;

            var currentAmmo = _activeAmmo;

            _onReloaded.Invoke();

            Do(() =>
            {
                _ammo.Handler.Reload(_ammo.Data[currentAmmo]);
                UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

                _state = Data.WeaponState.Idle;
                _stateInfo.Forget();
            }, Data.WeaponState.Reload, _ammo.Data[currentAmmo].ReloadTime);

            return true;
        }

        public void BreakAction()
        {
            if (_actionCoroutine == null) return;
            
            StopCoroutine(_actionCoroutine);
            _actionCoroutine = null;
                
            _stateInfo.CalculateRemaining();
        }

        public void ResumeAction()
        {
            if (!_stateInfo.IsEmpty)
                _actionCoroutine ??= StartCoroutine(ActionRoutine(0, _stateInfo.TimeRemaining));
        }

        public bool NextAmmoType()
        {
            if (!ChangeAmmoType(ChangeDirection.Forward)) return false;

            _onAmmoChanged.Invoke();

            return true;
        }

        public bool PreviousAmmoType()
        {
            if (!ChangeAmmoType(ChangeDirection.Back)) return false;

            _onAmmoChanged.Invoke();

            return true;
        }
        
        public bool NextShootingMode()
        {
            if (!ChangeShootingMode(ChangeDirection.Forward)) return false;

            _onShootingModeChanged.Invoke();

            return true;
        }

        public bool PreviousShootingMode()
        {
            if (!ChangeShootingMode(ChangeDirection.Back)) return false;

            _onShootingModeChanged.Invoke();

            return true;
        }

        public bool SetAmmoType(int index)
        {
            if (!ChangeAmmoType(index))
                return false;

            _onAmmoChanged.Invoke();

            return true;
        }

        public bool SetShootingMode(int index)
        {
            if (!ChangeShootingMode(index))
                return false;

            _onShootingModeChanged.Invoke();

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
