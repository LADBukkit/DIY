using DIY.Util;
using System;
using System.Collections.Generic;

namespace DIY.Filter
{
    /// <summary>
    /// Blueprint for the filters
    /// </summary>
    public abstract class Filter
    {
        /// <summary>
        /// The Name of the Filter
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// An array of all the properties
        /// Please set this one on the Constructor
        /// </summary>
        public FilterProperty[] Properties;

        /// <summary>
        /// Applies the Filter to a Bitmap
        /// Normally this Image clones the input before changing
        /// </summary>
        /// <param name="input">The Bitmap to filter upon</param>
        /// <returns></returns>
        public abstract DirectBitmap CalculateFilter(DirectBitmap input);
    }

    /// <summary>
    /// Normal Filter Property
    /// </summary>
    public abstract class FilterProperty
    {
        /// <summary>
        /// The Type of the Property
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The Name of the Property
        /// </summary>
        public string Name { get; }

        public FilterProperty(Type type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    /// <summary>
    /// A FilterProperty for numeric types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterPropertyNumeric<T> : FilterProperty
    {
        /// <summary>
        /// The Minimum value
        /// </summary>
        public T Min { get; }

        /// <summary>
        /// The Maximum Value
        /// </summary>
        public T Max { get; }

        /// <summary>
        /// The default Value
        /// </summary>
        public T Default { get; }

        /// <summary>
        /// The Interval for changing
        /// </summary>
        public T Interval { get; }

        /// <summary>
        /// The Current Value
        /// </summary>
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
