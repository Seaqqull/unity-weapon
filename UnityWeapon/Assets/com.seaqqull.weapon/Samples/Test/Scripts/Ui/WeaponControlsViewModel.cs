using UnityEngine;
using UnityEngine.UIElements;
using Weapons.Data;

namespace com.seaqqull.weapon.Samples.Test.Scripts.Ui
{
  [RequireComponent(typeof(UIDocument))]
  public class WeaponControlsViewModel : MonoBehaviour
  {
    [SerializeField] private Weapons.Weapon _weapon;
    [SerializeField] private WeaponControlsSO _weaponUIData;
    [Space]
    [SerializeField] private bool _hideActionProgress = true;

    private UIDocument _uiDocument;
    private ProgressBar _progress;
    private Button _shootButton;

    private bool _uiControlsResolved;


    private void Awake() =>
      _uiDocument = GetComponent<UIDocument>();

    private void Start()
    {
      _progress = _uiDocument.rootVisualElement.Q<ProgressBar>("ActionProgress");
      _shootButton = _uiDocument.rootVisualElement.Q<Button>("ShootAction");
    }

    private void OnEnable()
    {
      ResolveUIControls();

      _progress.dataSource = _weaponUIData;
      _shootButton.RegisterCallback<ClickEvent>(Shoot);

      _weaponUIData.ActionProgressDisplay = _hideActionProgress ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void OnDisable()
    {
      _progress.dataSource = null;
      _shootButton.UnregisterCallback<ClickEvent>(Shoot);
    }


    private void ResolveUIControls()
    {
      if (_uiControlsResolved)
        return;

      _progress = _uiDocument.rootVisualElement.Q<ProgressBar>("ActionProgress");
      _shootButton = _uiDocument.rootVisualElement.Q<Button>("ShootAction");

      _uiControlsResolved = true;
    }

    private void Shoot(ClickEvent e) =>
      _weapon.Shoot();


    public void OnWeaponActionProgress(float progress) =>
      _weaponUIData.ActionProgress = (int) Mathf.Lerp(0, 100, progress);

    public void OnWeaponStateChange(WeaponState state)
    {
      switch (state)
      {
        case WeaponState.Shooting:
        case WeaponState.Reload:
          _weaponUIData.ActionProgressDisplay = DisplayStyle.Flex;
          break;
        case WeaponState.None:
        case WeaponState.Idle:
        default:
          _weaponUIData.ActionProgressDisplay = _hideActionProgress ? DisplayStyle.None : DisplayStyle.Flex;
          break;
      }
    }
  }
}