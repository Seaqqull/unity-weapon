using UnityEngine;

namespace Attribute
{
    [CreateAssetMenu(menuName = "AttributeModifier/Create")]
    public class StaticModifier : ScriptableObject, Attribute.Data.IAttribute
    {
#pragma warning disable 0649
        [SerializeField] private Attribute.Data.Attribute _attribute;
#pragma warning restore 0649
        public Attribute.Data.Attribute Attribute
        {
            get { return _attribute; }
        }


        public static implicit operator Attribute.Data.Attribute(StaticModifier obj)
        {
            return obj.Attribute;
        }
    }

    [System.Serializable]
    public class StaticAttributes : Attribute.Data.AttributesController<StaticModifier> { }
}