using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.seaqqull.weapon.Samples.Test.Scripts.Ui
{
  [CreateAssetMenu(menuName = "Samples/Ui/WeaponControls")]
  public class WeaponControlsSO : ScriptableObject
  {
    private int _actionProgress;

    [CreateProperty]
    public int ActionProgress
    {
      get => _actionProgress;
      set
      {
        if (_actionProgress == value)
          return;

        _actionProgress = value;
        ActionProgressDisplay = _actionProgress > 0 ? DisplayStyle.Flex : DisplayStyle.None;
      }
    }

    [field: SerializeField, DontCreateProperty]
    [CreateProperty]
    public DisplayStyle ActionProgressDisplay { get; set; }
  }
}