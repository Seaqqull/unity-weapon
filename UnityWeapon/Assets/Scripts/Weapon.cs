using System.Collections;
using UnityEngine;
using System;


namespace Weapons
{
    public class Weapon : BaseMonoBehaviour, Utility.IRunLater
    {
        #region InnerClasses
        [Flags]
        public enum UIUpdateMode { None, MagazineAmount, OverallAmount, Progress = 4 }
        [System.Serializable]
        public class AmmoComputeData : Utility.ComputingData<Ammo.AmmoHandler, Ammo.AmmoController> { }
        [System.Serializable]
        public class ShootingComputeObject : Utility.ComputingData<Shooting.ShootingHandler, Shooting.ShootingMode> { }
        #endregion

        #region EditorVariables
        [SerializeField] private Data.WeaponType _type;
        [Header("Aiming")]
        [SerializeField] protected int _flowLengthDraw;
        [SerializeField] protected Transform _bulletFlowPath;
        [SerializeField] protected Aiming.Accuracy _accuracy;
        [Header("Responces")]
        [SerializeField] protected Data.ActionResponce _onShot;
        [SerializeField] protected Data.ActionResponce _onReload;
        [SerializeField] protected Data.ActionResponce _onChangeAmmo;
        [SerializeField] protected Data.ActionResponce _onChangeShootingMode;
        [Header("Ammo")]
        [SerializeField] protected int _activeAmmo;
        [SerializeField] protected AmmoComputeData _ammo;
        [Header("Shooting")]
        [SerializeField] protected int _activeShooting;
        [SerializeField] protected ShootingComputeObject _shooting;
        #endregion

        #region Variables
        protected Coroutine _progressCoroutine;
        protected Coroutine _actionCoroutine;

        protected Action<Weapon> _onMagazineAmmoChangeUI;
        protected Action<float> _onActionProgress;
        protected Action<Weapon> _onAmmoChangeUI;

        protected Data.FloatRange _actionProgress;

        protected Data.StateInfo _stateInfo;
        protected Action _stateResultAciton;
        protected Data.WeaponState _state;
        #endregion

        #region Fields
        public event Action<Weapon> OnUIMagazineAmmoChange
        {
            add { _onMagazineAmmoChangeUI += value; }
            remove { _onMagazineAmmoChangeUI -= value; }
        }
        public event Action<float> OnActionProgress
        {
            add { _onActionProgress += value; }
            remove { _onActionProgress -= value; }
        }
        public event Action<Weapon> OnUIAmmoChange
        {
            add { _onAmmoChangeUI += value; }
            remove { _onAmmoChangeUI -= value; }
        }
        public event Action OnChangeShootingMode
        {
            add { _onChangeShootingMode.CodeResponce += value; }
            remove { _onChangeShootingMode.CodeResponce -= value; }
        }
        public event Action OnChangeAmmo
        {
            add { _onChangeAmmo.CodeResponce += value; }
            remove { _onChangeAmmo.CodeResponce -= value; }
        }
        public event Action OnReload
        {
            add { _onReload.CodeResponce += value; }
            remove { _onReload.CodeResponce -= value; }
        }
        public event Action OnShot
        {
            add { _onShot.CodeResponce += value; }
            remove { _onShot.CodeResponce -= value; }
        }

        public Ammo.AmmoHandler AmmoHandler
        {
            get { return _ammo.Handler; }
        }
        public Shooting.ShootingMode Mode
        {
            get { return _shooting.Data[_activeShooting]; }
        }
        public Aiming.Accuracy Accuracy
        {
            get { return this._accuracy; }
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
            _stateInfo = new Data.StateInfo();

            ReloadInstant();
        }

        protected void OnDrawGizmos()
        {
            if (_bulletFlowPath == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_bulletFlowPath.position, _bulletFlowPath.position + _bulletFlowPath.forward * _flowLengthDraw);
        }

        protected void OnEnable()
        {
            if ((_stateInfo.IsEmpty) || 
                (_stateInfo.TimeRemaining == 0.0f)) return;

            _state = (Data.WeaponState)_stateInfo.Id;
            
            _progressCoroutine =
                StartCoroutine(UpdateProgress(0.0f, _stateInfo.TimeRemaining));

            _actionCoroutine = RunLaterValued(_stateResultAciton,
                _stateInfo.TimeRemaining);
        }

        protected void OnDisable()
        {
            if (_stateInfo.IsEmpty) return;

            _stateInfo.CalculateRemaining();
        }


        protected bool ReloadInstant()
        {
            if (!IsReloadPossible())
                return false;

            _ammo.Handler.Reload(_ammo.Data[_activeAmmo]);

            return true;
        }

        private bool ChangeAmmoType(bool modeDestination)
        {
            if ((_actionCoroutine != null) ||
                (_ammo.Data.Count <= 1))
                return false;

            int newIndex = (modeDestination) ?
                _activeAmmo + 1 : _activeAmmo - 1;

            if ((newIndex < 0) ||
                (newIndex >= _ammo.Data.Count))
            {
                newIndex = (modeDestination) ? 0 : _ammo.Data.Count - 1;
            }

            _activeAmmo = newIndex;

            return true;
        }

        private bool ChangeShootingMode(bool modeDestination)
        {
            if ((_actionCoroutine != null) ||
                (_shooting.Data.Count <= 1))
                return false;

            int newIndex = (modeDestination) ?
                _activeShooting + 1 : _activeShooting - 1;

            if ((newIndex < 0) ||
                (newIndex >= _shooting.Data.Count))
            {
                newIndex = (modeDestination) ? 0 : _shooting.Data.Count - 1;
            }

            _activeShooting = newIndex;

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

            _shooting.Data[currentSMode].Perform(this);

            _onShot.UiResponce?.Invoke();
            _onShot.CodeResponce?.Invoke();


            _state = Data.WeaponState.Shooting;
            _stateInfo.Remember((int)_state, _shooting.Data[currentSMode].TimeBetweenShot);

            _progressCoroutine =
                StartCoroutine(UpdateProgress(0, _stateInfo.TimeRemaining));

            UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

            _stateResultAciton = () =>
            {
                _actionCoroutine = null;

                _state = Data.WeaponState.Idle;
                _stateInfo.Forget();
            };

            _actionCoroutine = RunLaterValued(_stateResultAciton,
                _stateInfo.TimeRemaining);


            return true;
        }

        public bool Reload()
        {
            if (!IsReloadPossible()) return false;

            int currentAmmo = _activeAmmo;

            _onReload.UiResponce?.Invoke();
            _onReload.CodeResponce?.Invoke();

            _state = Data.WeaponState.Reload;
            _stateInfo.Remember((int)_state, _ammo.Data[currentAmmo].ReloadTime);

            _progressCoroutine =
                StartCoroutine(UpdateProgress(0, _stateInfo.TimeRemaining));

            _stateResultAciton = () =>
            {
                _ammo.Handler.Reload(_ammo.Data[currentAmmo]); // Was _activeAmmo
                UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

                _actionCoroutine = null;

                _state = Data.WeaponState.Idle;
                _stateInfo.Forget();
            };

            _actionCoroutine = RunLaterValued(_stateResultAciton,
                _stateInfo.TimeRemaining);


            return true;
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

        public bool IsReloadPossible()
        {
            if ((_actionCoroutine != null) ||
                        !_ammo.Handler.IsReloadPossible(_ammo.Data[_activeAmmo]))
                return false;
            return true;
        }


        public bool NextAmmoType()
        {
            if (!ChangeAmmoType(true))
                return false;

            _onChangeAmmo.UiResponce?.Invoke();
            _onChangeAmmo.CodeResponce?.Invoke();

            return true;
        }

        public bool PreviousAmmoType()
        {
            if (!ChangeAmmoType(false))
                return false;

            _onChangeAmmo.UiResponce?.Invoke();
            _onChangeAmmo.CodeResponce?.Invoke();

            return true;
        }
        
        public bool NextShootingMode()
        {
            if (!ChangeShootingMode(true))
                return false;

            _onChangeShootingMode.UiResponce?.Invoke();
            _onChangeShootingMode.CodeResponce?.Invoke();

            return true;
        }

        public bool PreviousShootingMode()
        {
            if (!ChangeShootingMode(false))
                return false;

            _onChangeShootingMode.UiResponce?.Invoke();
            _onChangeShootingMode.CodeResponce?.Invoke();

            return true;
        }

        public bool ChangeAmmoType(int index)
        {
            if ((_actionCoroutine != null) ||
                ((index < 0) || (index >= _ammo.Data.Count)))
                return false;
            
            _activeAmmo = index;

            _onChangeAmmo.UiResponce?.Invoke();
            _onChangeAmmo.CodeResponce?.Invoke();

            return true;
        }

        public bool ChangeShootingMode(int index)
        {
            if ((_actionCoroutine != null) ||
                ((index < 0) || (index >= _shooting.Data.Count)))
                return false;

            _activeShooting = index;

            _onChangeShootingMode.UiResponce?.Invoke();
            _onChangeShootingMode.CodeResponce?.Invoke();

            return true;
        }


        public void UpdateUI(UIUpdateMode uiUpdateMode)
        {
            if ((uiUpdateMode & UIUpdateMode.OverallAmount) != 0)
                _onAmmoChangeUI?.Invoke(this);
            if ((uiUpdateMode & UIUpdateMode.MagazineAmount) != 0)
                _onMagazineAmmoChangeUI?.Invoke(this);
            if ((uiUpdateMode & UIUpdateMode.Progress) != 0)
                _onActionProgress?.Invoke(_actionProgress.GetAverage());
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
