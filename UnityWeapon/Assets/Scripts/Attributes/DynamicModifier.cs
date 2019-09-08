using UnityEngine;

namespace Attribute
{
    [System.Serializable]
    public class DynamicModifier : Attribute.Data.IAttribute
    {
#pragma warning disable 0649
        [SerializeField] private Attribute.Data.Attribute _attribute;
#pragma warning restore 0649
        public Attribute.Data.Attribute Attribute
        {
            get { return _attribute; }
        }


        public static implicit operator Attribute.Data.Attribute(DynamicModifier obj)
        {
            return obj.Attribute;
        }
    }

    [System.Serializable]
    public class DynamicAttributes : Attribute.Data.AttributesController<DynamicModifier> { }
}