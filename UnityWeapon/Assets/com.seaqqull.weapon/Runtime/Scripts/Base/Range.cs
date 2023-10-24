namespace Weapon.Base
{
    public abstract class Range<T>
    {
        public abstract float Progress { get; }

        public T Value;
        public T Min;
        public T Max;

        public void Reset()
        {
            Value = default;
            Max = default;
            Min = default;
        }

        public void Update(T value, T min, T max)
        {
            Value = value;
            Max = max;
            Min = min;
        }
    }

}