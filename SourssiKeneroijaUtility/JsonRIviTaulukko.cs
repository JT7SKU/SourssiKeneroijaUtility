using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SourssiKeneroijaUtility.JSONRivit
{
    public readonly struct JsonRIviTaulukko<T> : IEquatable<JsonRIviTaulukko<T>>, IReadOnlyCollection<T>
     where T : IEquatable<T>
    {
        /// <summary>
        /// The underlying <typeparamref name="T"/> array.
        /// </summary>
        private readonly T[]? _taulukko;

        /// <summary>
        /// Creates a new <see cref="EquatableArray{T}"/> instance.
        /// </summary>
        /// <param name="Taulukko">The input <see cref="ImmutableArray{T}"/> to wrap.</param>
        public JsonRIviTaulukko(T[] Taulukko)
        {
            _taulukko = Taulukko;
        }

        public bool Equals(JsonRIviTaulukko<T> taulukko)
        {
            return AsSpan().SequenceEqual(taulukko.AsSpan());
        }

        public ReadOnlySpan<T> AsSpan()
        {
            return _taulukko.AsSpan();
        }

        /// <sinheritdoc/>
        public override int GetHashCode()
        {
            if (_taulukko is not T[] taulukko)
            {
                return 0;
            }

            HashCode hashCode = default;

            foreach (T item in taulukko)
            {
                hashCode.Add(item);
            }

            return hashCode.ToHashCode();
        }
        public int Count => _taulukko?.Length ?? 0;

        /// <sinheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is JsonRIviTaulukko<T> taulukko && this.Equals(taulukko);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)(_taulukko ?? Array.Empty<T>())).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)(_taulukko ?? Array.Empty<T>())).GetEnumerator();
        }
        public static bool operator ==(JsonRIviTaulukko<T> left, JsonRIviTaulukko<T> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(JsonRIviTaulukko<T> left, JsonRIviTaulukko<T> right)
        {
            return !left.Equals(right);
        }
    }
}
