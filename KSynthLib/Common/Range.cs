using System;
using System.Collections.Generic;

// This file contains the essence of Range.NET (https://github.com/mnelsonwhite/Range.NET),
// licensed under the MIT License.
// The reason for incorporating the types is that the NuGet library has a dependency
// on "netcoreapp2.1", whereas the KSynthLib library is built for "netstandard2.1".

namespace KSynthLib.Common
{
    public enum RangeInclusivity
    {
        ExclusiveMinExclusiveMax = 0,
        ExclusiveMinInclusiveMax = 1,
        InclusiveMinExclusiveMax = 2,
        InclusiveMinInclusiveMax = 3
    }

    public interface IEnumerableRange<T> : IRange<T>, IEnumerable<T> where T : IComparable<T> { }

    public interface IRange<out T> where T : IComparable<T>
    {
        T Minimum { get; }
        T Maximum { get; }
        RangeInclusivity Inclusivity { get; }
    }

    /// <summary>
    /// Generic range class.
    /// Inclusivity is set for min and max by default
    /// </summary>
    /// <typeparam name="T">Constrained to IComparable</typeparam>
    public sealed class Range<T> : IRange<T> where T : IComparable<T>
    {
        private T _minimum;
        private T _maximum;

        /// <summary>
        /// Default Inclusivity is set to InclusiveMinInclusiveMax
        /// </summary>
        public Range()
        {
            Inclusivity = RangeInclusivity.InclusiveMinInclusiveMax;
        }

        /// <param name="minimum">Minimum value</param>
        /// <param name="maximum">Maximum value</param>
        public Range(T minimum, T maximum) : this()
        {
            var reverse = minimum.CompareTo(maximum) > 0;
            _minimum = reverse ? maximum : minimum;
            _maximum = reverse ? minimum : maximum;
        }

        /// <summary>
        /// Minimum value of the range
        /// </summary>
        public T Minimum
        {
            get => _minimum;
            set
            {
                if (value.CompareTo(Maximum) > 0)
                {
                    _minimum = _maximum;
                    _maximum = value;
                }
                else
                {
                    _minimum = value;
                }
            }
        }

        /// <summary>
        /// Maximum value of the range
        /// </summary>
        public T Maximum
        {
            get => _maximum;
            set
            {
                if (Minimum.CompareTo(value) > 0)
                {
                    _maximum = _minimum;
                    _minimum = value;
                }
                else
                {
                    _maximum = value;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the inclusivity of the range
        /// </summary>
        public RangeInclusivity Inclusivity { get; set; }

        /// <summary>
        /// Presents the Range in readable format
        /// </summary>
        /// <returns>String representation of the Range</returns>
        public override string ToString()
        {
            return $"[{Minimum} - {Maximum}]";
        }

        /// <summary>
        /// Override of GetHashCode to allow equality
        /// </summary>
        /// <returns>integer hash code representing the value of this instance</returns>
        public override int GetHashCode() => HashCode.Combine(Minimum, Maximum, Inclusivity);

        /// <summary>
        /// Use the overridden GetHashCode method to test equality
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if both object have the same hash code value</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;

            var val = obj as Range<T>;
            return !ReferenceEquals(val, null) &&
                Minimum.CompareTo(val.Minimum) == 0 &&
                Maximum.CompareTo(val.Maximum) == 0 &&
                Equals(Inclusivity, val.Inclusivity);
        }
    }

public static class RangeExtensions
    {
        /// <summary>
        /// Determines if the provided value is inside the range
        /// </summary>
        /// <param name="range">The range to test</param>
        /// <param name="value">The value to test</param>
        /// <returns>True if the value is inside Range, else false</returns>
        public static bool Contains<T>(this IRange<T> range, T value)
            where T : IComparable<T>
        {
            var minInclusive = ((int) range.Inclusivity & 2) == 2; // If the second bit set then min is inclusive
            var maxInclusive = ((int) range.Inclusivity & 1) == 1; // If the first bit set then max is inclusive

            var testMin = minInclusive ? range.Minimum.CompareTo(value) <= 0 : range.Minimum.CompareTo(value) < 0;
            var testMax = maxInclusive ? range.Maximum.CompareTo(value) >= 0 : range.Maximum.CompareTo(value) > 0;

            return testMin && testMax;
        }

        /// <summary>
        /// Determines if another range is inside the bounds of this range
        /// </summary>
        /// <param name="range">The range to test</param>
        /// <param name="value">The value to test</param>
        /// <returns>True if range is inside, else false</returns>
        public static bool Contains<T>(this IRange<T> range, IRange<T> value)
            where T : IComparable<T>
        {
            return range.Contains(value.Minimum) // For when A contains B
                   || range.Contains(value.Maximum)
                   || value.Contains(range.Minimum) // For when B contains A
                   || value.Contains(range.Maximum);
        }

        /// <summary>
        /// Determines if another range intersects with this range.
        /// The either range may completely contain the other
        /// </summary>
        /// <param name="range">The range</param>
        /// <param name="value">The other range</param>
        /// <returns>True of the this range intersects the other range</returns>
        public static bool Intersects<T>(this IRange<T> range, IRange<T> value)
            where T : IComparable<T>
        {
            return
                range.Contains(value.Minimum) || // For when A contains B
                range.Contains(value.Maximum) ||
                value.Contains(range.Minimum) || // For when B contains A
                value.Contains(range.Maximum);
        }

        /// <summary>
        /// Create a union of two ranges so that a new range with the minimum of
        /// the minimum of both ranges and the maximum of the maximum of bother ranges
        /// </summary>
        /// <param name="range">A range with which to union</param>
        /// <param name="value">A range with which to union</param>
        /// <returns>A new range with the minimum of the minimum of both ranges and
        /// the maximum of the maximum of bother ranges</returns>
        public static IRange<T> Union<T>(this IRange<T> range, IRange<T> value)
            where T : IComparable<T>
        {
            return new Range<T>(
                range.Minimum.CompareTo(value.Minimum) < 0 ? range.Minimum : value.Minimum,
                range.Maximum.CompareTo(value.Maximum) > 0 ? range.Maximum : value.Maximum);
        }
    }
}
