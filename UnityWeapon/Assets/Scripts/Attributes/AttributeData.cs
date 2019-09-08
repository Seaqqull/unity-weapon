using System.Collections.Generic;
using UnityEngine;

namespace Attribute.Data
{
    public enum ValueType { Relative, Absolute }

    public interface IAttribute
    {
        Attribute Attribute { get; }
    }
    public interface IAttributeContainer
    {
        float GetAttributeAbsolute(string key);
        float GetAttributeRelative(string key);
    }

    [System.Serializable]
    public struct Attribute
    {
        public StringReference Name;
        public ValueType Type;
        [Range(0, ushort.MaxValue)] public float Value;
    }

    public class AttributesController<T> where T : IAttribute
    {
#pragma warning disable 0649
        [SerializeField] private List<T> _items;
#pragma warning restore 0649

        private Dictionary<string, float> _valuesAbsolute;
        private Dictionary<string, float> _valuesRelative;

        private IReadOnlyDictionary<string, float> _absolutes;
        private IReadOnlyDictionary<string, float> _relatives;        

        public IReadOnlyDictionary<string, float> Absolutes
        {
            get { return this._absolutes; }
        }
        public IReadOnlyDictionary<string, float> Relatives
        {
            get { return this._relatives; }
        }


        private void AddAbsolute(Attribute attribute)
        {
            if (!_valuesAbsolute.ContainsKey(attribute.Name))
                _valuesAbsolute.Add(attribute.Name, attribute.Value);
            else
            {
                _valuesAbsolute[attribute.Name] =
                    _valuesAbsolute[attribute.Name] + attribute.Value;
            }
        }

        private void AddRelative(Attribute attribute)
        {
            if (!_valuesRelative.ContainsKey(attribute.Name))
                _valuesRelative.Add(attribute.Name, attribute.Value);
            else
            {
                _valuesRelative[attribute.Name] =
                    _valuesRelative[attribute.Name] * attribute.Value;
            }            
        }


        public void BakeAttributes()
        {
            IAttribute[] attributes = 
                System.Array.ConvertAll(_items.ToArray(), item => (IAttribute)item);

            _valuesAbsolute = new Dictionary<string, float>();
            _valuesRelative = new Dictionary<string, float>();
            _absolutes = _valuesAbsolute;
            _relatives = _valuesRelative;

            for (int i = 0; i < attributes.Length; i++)
            {
                IAttribute attribute = attributes[i];
                
                if (attribute.Attribute.Type == ValueType.Absolute)
                    AddAbsolute(attribute.Attribute);
                else
                    AddRelative(attribute.Attribute);
            }
        }

        public float GetValue(string key)
        {
            if ((key == null) || (key == string.Empty)) return 0.0f;

            _valuesAbsolute.TryGetValue(key, out float valueAbsolute);
            bool relativeExist = 
                _valuesRelative.TryGetValue(key, out float valueRelative);

            return (relativeExist) ? valueAbsolute * valueRelative : valueAbsolute;
        }
    }
}
