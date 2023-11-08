using Weapons.Shooting;
using Weapons.Ammo;
using Weapons.Data;
using Weapon.Base;
using System;


namespace Weapons
{
    public class RobustWeapon : IWeapon
    {
        private FloatRange _actionProgress = new();
        private StateInfo _stateInfo = new();
        private float _actionTimeRemaining;
        
        private IShootingHandler _shootingHandler;
        private IAmmoHandler _ammoHandler;
        private IAmmoController[] _ammo;
        private IShootingMode[] _modes;

        private WeaponState _state;
        private WeaponType _type;
        private string _name;

        private Action _stateResultAction;
        private int _activeShooting;
        private int _activeAmmo;

        public event Action<IWeapon> OnMagazineAmmoChangedUI;
        public event Action<WeaponState> OnStateChanged;
        public event Action<IWeapon> OnAmmoChangedUI;
        public event Action<float> OnStateProgressUI;
        public event Action OnShootingModeChanged;
        public event Action OnAmmoChanged;
        public event Action OnReloaded;
        public event Action OnShot;
        
        public bool IsActionExecutable => _stateInfo.IsEmpty;
        public bool IsReloadPossible =>
            IsActionExecutable && _ammoHandler.IsReloadPossible(Ammo);
        public bool IsMagazineEmpty => Ammo.MagazineAmount == 0;
        public bool IsShotPossible =>
            IsActionExecutable && ShootingHandler.IsExecutable(this) && Mode.IsExecutable(this);

        public WeaponState State
        {
            get => _state;
            private set
            {
                _state = value;
                OnStateChanged?.Invoke(State);
            }
        }

        public WeaponType Type => _type;
        public string Name => _name;
        
        public IShootingHandler ShootingHandler => _shootingHandler;
        public IShootingMode Mode => _modes[_activeShooting];
        public IAmmoController Ammo => _ammo[_activeAmmo];
        public IAmmoHandler AmmoHandler => _ammoHandler;


        public RobustWeapon(string name, WeaponType type, IAmmoHandler ammoHandler, IAmmoController[] ammo, IShootingHandler shootingHandler, IShootingMode[] modes)
        {
            _shootingHandler = shootingHandler;
            _ammoHandler = ammoHandler;
            _modes = modes;
            _ammo = ammo;
            
            _name = name;
            _type = type;
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


        private bool ChangeAmmoType(int modeIndex)
        {
            if (!IsActionExecutable || modeIndex < 0 || modeIndex >= _ammo.Length) return false;

            _activeAmmo = modeIndex;
            return true;
        }
        
        private bool ChangeShootingMode(int modeIndex)
        {
            if (!IsActionExecutable || modeIndex < 0 || modeIndex >= _modes.Length) return false;
            
            _activeShooting = modeIndex;
            return true;
        }

        private void UpdateUI(UIUpdateMode uiUpdateMode)
        {
            if (uiUpdateMode.HasFlag(UIUpdateMode.OverallAmount))
                OnAmmoChangedUI?.Invoke(this);
            if (uiUpdateMode.HasFlag(UIUpdateMode.MagazineAmount))
                OnMagazineAmmoChangedUI?.Invoke(this);
            if (uiUpdateMode.HasFlag(UIUpdateMode.Progress))
                OnStateProgressUI?.Invoke(_actionProgress.Progress);
        }
        
        private bool ChangeAmmoType(ChangeDirection direction)
        {
            if (!IsActionExecutable) return false;
            if (_ammo.Length <= 1) return true;

            _activeAmmo = GetFromDirection(direction, _activeAmmo, 1, 0, _ammo.Length - 1);
            return true;
        }
        
        private bool ChangeShootingMode(ChangeDirection direction)
        {
            if (!IsActionExecutable) return false; 
            if (_modes.Length <= 1) return true;

            _activeShooting = GetFromDirection(direction, _activeShooting, 1, 0, _modes.Length - 1);
            return true;
        }

        private void Do(Action action, WeaponState state, float period)
        {
            State = state;
            _stateInfo.Remember((int)state, period);

            _actionProgress.Update(0, 0, period);
            _stateResultAction = action;
        }
        

        public void BreakAction()
        {
            if (!_stateInfo.IsEmpty) 
                _stateInfo.CalculateRemaining(_actionProgress.Progress);
        }

        public void ResumeAction()
        {
            if (!_stateInfo.IsEmpty)
                _actionProgress.Update(0, 0, _stateInfo.TimeRemaining);
        }
        
        public void Process(float deltaTime)
        {
            if (_stateInfo.IsEmpty)
                return;
            
            _actionProgress.Value += deltaTime;
            if (!_actionProgress.IsReached)
            {
                UpdateUI(UIUpdateMode.Progress);
                return;
            }
            
            _actionProgress.Value = _actionProgress.Max;
            UpdateUI(UIUpdateMode.Progress);
            _actionProgress.Reset();

            _stateResultAction?.Invoke();
            _stateResultAction = null;
        }

        public bool Shoot()
        {
            if (!IsShotPossible) return false;

            Mode.Perform(this);
            OnShot?.Invoke();

            Do(() =>
            {
                _stateInfo.Forget();
                State = WeaponState.Idle;
            }, WeaponState.Shooting, Mode.TimeBetweenShot);
            UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

            return true;
        }

        public bool Reload()
        {
            if (!IsReloadPossible) return false;

            var currentAmmo = _activeAmmo;
            OnReloaded?.Invoke();

            Do(() =>
            {
                _ammoHandler.Reload(_ammo[currentAmmo]);
                UpdateUI(UIUpdateMode.MagazineAmount | UIUpdateMode.OverallAmount);

                _stateInfo.Forget();
                State = WeaponState.Idle;
            }, WeaponState.Reload, Ammo.ReloadTime);

            return true;
        }

        public void MakeShot()
        {
            Shoot();
        }

        public bool NextAmmoType()
        {
            if (!ChangeAmmoType(ChangeDirection.Forward)) return false;

            OnAmmoChanged?.Invoke();
            return true;
        }

        public bool PreviousAmmoType()
        {
            if (!ChangeAmmoType(ChangeDirection.Back)) return false;

            OnAmmoChanged?.Invoke();
            return true;
        }
        
        public bool NextShootingMode()
        {
            if (!ChangeShootingMode(ChangeDirection.Forward)) return false;

            OnShootingModeChanged?.Invoke();
            return true;
        }

        public bool PreviousShootingMode()
        {
            if (!ChangeShootingMode(ChangeDirection.Back)) return false;

            OnShootingModeChanged?.Invoke();
            return true;
        }

        public bool SetAmmoType(int index)
        {
            if (!ChangeAmmoType(index))
                return false;

            OnAmmoChanged?.Invoke();
            return true;
        }

        public bool SetShootingMode(int index)
        {
            if (!ChangeShootingMode(index))
                return false;

            OnShootingModeChanged?.Invoke();
            return true;
        }
    }
}