using System.Collections.Generic;
using UnityEngine;
using Utilities.Properties;

namespace Attribute.Data
{
    public enum AttributeType { Relative, Absolute }

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
        public AttributeType Type;
        [Range(0, ushort.MaxValue)] public float Value;
    }

    public class AttributesController<T> where T : IAttribute
    {
#pragma warning disable 0649
        [SerializeField] private List<T> _items = new();
#pragma warning restore 0649

        private Dictionary<string, float> _valuesAbsolute;
        private Dictionary<string, float> _valuesRelative;

        private IReadOnlyDictionary<string, float> _absolutes;
        private IReadOnlyDictionary<string, float> _relatives;        

        public IReadOnlyDictionary<string, float> Absolutes => _absolutes;
        public IReadOnlyDictionary<string, float> Relatives => _relatives;


        private void Add(Attribute attribute)
        {
            var values = attribute.Type == AttributeType.Absolute ? _valuesAbsolute : _valuesRelative;
            
            if (!values.ContainsKey(attribute.Name))
                values.Add(attribute.Name, attribute.Value);
            else
                values[attribute.Name] =
                    attribute.Type == AttributeType.Absolute 
                        ? values[attribute.Name] + attribute.Value 
                        : values[attribute.Name] * attribute.Value;
        }

        public void BakeAttributes()
        {
            _valuesAbsolute = new Dictionary<string, float>();
            _valuesRelative = new Dictionary<string, float>();
            _absolutes = _valuesAbsolute;
            _relatives = _valuesRelative;

            foreach (var attribute in _items)
                Add(attribute.Attribute);
        }

        public float GetValue(string key)
        {
            if (string.IsNullOrEmpty(key)) return 0.0f;

            _valuesAbsolute.TryGetValue(key, out var valueAbsolute);
            return _valuesRelative.TryGetValue(key, out var valueRelative)
                ? valueAbsolute * valueRelative
                : valueAbsolute;
        }
    }
}
