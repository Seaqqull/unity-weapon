using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;
using Weapon.Base;
using Weapon.Storage;
using Weapon.Storage.Data;
using Weapons.Ammo;
using Weapons.Data;
using Weapons.Shooting;


namespace Weapons
{
    public class Weapon : BaseMonoBehaviour, IGameObjectWeapon, global::Weapon.Utility.IRunLater
    {
        #region Constants
        private static readonly string BULLETS_CONTAINER = new("Bullets");
        #endregion

        #region EditorVariables
        [SerializeField] private WeaponType _type;
        [SerializeField] private Aiming.Accuracy _accuracy;
        [Header("Initialization")] 
        [SerializeField] private bool _reloadOnStartup;
        [Space]
        [SerializeField] private int _poolAmount;
        [SerializeField] private int _expansionAmount;
        [SerializeField] private int _reductionAmount;
        [Header("Events")] 
        [SerializeField] private UnityEvent _onShot;
        [SerializeField] private UnityEvent _onReloaded;
        [SerializeField] private UnityEvent _onAmmoChanged;
        [SerializeField] private UnityEvent _onShootingModeChanged;
        [SerializeField] private UnityEvent<WeaponState> _onStateChanged;
        [Space]
        [SerializeField] private UiUpdateRate _uiUpdateRate = UiUpdateRate.FPS_Umlimited;
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
        protected FloatRange _actionProgress = new();
        protected StateInfo _stateInfo = new();
        protected WaitForSeconds _uiUpdateTime;
        protected Coroutine _actionCoroutine;
        protected GameObject _bulletsStorage;
        protected Action _stateResultAction;
        protected WeaponState _state;
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

        public IShootingMode Mode => _shooting.Data[_activeShooting];
        public IShootingHandler ShootingHandler => _shooting.Handler;
        public IAmmoController Ammo => _ammo.Data[_activeAmmo];
        public IAmmoHandler AmmoHandler => _ammo.Handler;
        public Transform BulletFlow => _accuracy.Flow;
        public Aiming.Accuracy Accuracy => _accuracy;
        public string Name => GameObj.name;
        public WeaponType Type => _type;

        public bool IsActionExecutable => _actionCoroutine == null;
        public bool IsReloadPossible => 
            IsActionExecutable && 
            AmmoHandler.IsReloadPossible(_ammo.Data[_activeAmmo]);
        public bool IsMagazineEmpty => (Ammo.MagazineAmount == 0);
        public bool IsShotPossible =>
            IsActionExecutable &&
            ShootingHandler.IsExecutable(this) &&
            Mode.IsExecutable(this);
        public UiUpdateRate UiUpdateRate
        {
            get => _uiUpdateRate;
            set
            {
                _uiUpdateRate = value;
                _uiUpdateTime = (_uiUpdateRate == UiUpdateRate.FPS_Umlimited) 
                    ? null 
                    :  new WaitForSeconds(1.0f / (int) UiUpdateRate);
            }
        }
        public WeaponState State
        {
            get => _state;
            protected set
            {
                _state = value;
                _onStateChanged.Invoke(_state);
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
            {
                _storage.Populate(
                    _ammo.Data[i].Bullet.BulletObject,
                    new StorageData(i, _poolAmount, _expansionAmount, _reductionAmount));
            }

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


        private static int GetFromDirection(ChangeDirection direction, int current, int step, int leftBound, int rightBound)
        {
            return direction switch
            {
                ChangeDirection.Back => (current < leftBound)
                    ? rightBound - (leftBound - (current - step))
                    : current - step,
                ChangeDirection.Forward => (current > rightBound)
                    ? leftBound + ((current + step) - rightBound)
                    : current + step,
                _ => current
            };
        }


        private bool ReloadInstantly()
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
        
        private void UpdateUI(UIUpdateMode uiUpdateMode)
        {
            if (uiUpdateMode.HasFlag(UIUpdateMode.OverallAmount))
                _onAmmoChangedUI.Invoke(this);
            if (uiUpdateMode.HasFlag(UIUpdateMode.MagazineAmount))
                _onMagazineAmmoChangedUI.Invoke(this);
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

        private IEnumerator ActionRoutine(float min, float max)
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

        private void Do(Action action, WeaponState state, float period)
        {
            State = state;
            _stateInfo.Remember((int)state, period);

            _actionCoroutine = StartCoroutine(ActionRoutine(0, period));
            _stateResultAction = action;
        }


        public bool Shoot()
        {
            if (!IsShotPossible) return false;

            Mode.Perform(this);
            _onShot.Invoke();

            Do(() =>
            {
                _stateInfo.Forget();
                State = WeaponState.Idle;
            }, WeaponState.Shooting, Mode.TimeBetweenShot);
            UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

            return true;
        }

        [ContextMenu("Clear sample direction")]
        public void MakeShot()
        {
            Shoot();
        }

        public GameObject NextBullet()
        {
            return _storage.Pool(_activeAmmo);
        }

        // public void LockShoot(Action<bool> shotCallback)
        // {
        //     StartCO
        // }
        //
        // public void UnlockShoot()
        // {
        //     
        // }

        public bool Reload()
        {
            if (!IsReloadPossible) return false;

            var currentAmmo = _activeAmmo;
            _onReloaded.Invoke();

            Do(() =>
            {
                _ammo.Handler.Reload(_ammo.Data[currentAmmo]);
                UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

                _stateInfo.Forget();
                State = WeaponState.Idle;
            }, WeaponState.Reload, _ammo.Data[currentAmmo].ReloadTime);

            return true;
        }

        public void BreakAction()
        {
            if (_actionCoroutine == null) return;
            
            StopCoroutine(_actionCoroutine);
            _actionCoroutine = null;
                
            _stateInfo.CalculateRemaining(_actionProgress.Progress);
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
