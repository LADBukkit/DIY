using DIY.Util;
using System;
using System.Collections.Generic;

namespace DIY.Filter
{
    public abstract class Filter
    {
        public abstract string Name { get; }
        public FilterProperty[] Properties;
        public abstract DirectBitmap CalculateFilter(DirectBitmap input);
    }

    public abstract class FilterProperty
    {
        public Type Type { get; }
        public string Name { get; }

        public FilterProperty(Type type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    public class FilterPropertyNumeric<T> : FilterProperty
    {
        public T Min { get; }
        public T Max { get; }
        public T Default { get; }
        public T Interval { get; }

        public T Value { get; set; }

        public FilterPropertyNumeric(string name, T min, T max, T def, T interval) : base(typeof(T), name) {
            Min = min;
            Max = max;
            Default = def;
            Value = def;
            Interval = interval;
        }
    }
}
