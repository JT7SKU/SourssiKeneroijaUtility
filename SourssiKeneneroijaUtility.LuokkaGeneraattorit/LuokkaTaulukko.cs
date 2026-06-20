using System.Collections;

namespace SourssiKeneroijaUtility.LuokkaGeneraattorit
{
    public readonly struct LuokkaTaulukko<T> : IEquatable<LuokkaTaulukko<T>>, IReadOnlyCollection<T>
     where T : IEquatable<T>
    {
        private readonly T[]? _taulukko;

        public LuokkaTaulukko(T[]? Taulukko)
        {
            _taulukko = Taulukko;
        }
        public bool Equals(LuokkaTaulukko<T> taulukko)
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
            return obj is LuokkaTaulukko<T> taulukko && this.Equals(taulukko);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)(_taulukko ?? Array.Empty<T>())).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)(_taulukko ?? Array.Empty<T>())).GetEnumerator();
        }
        public static bool operator ==(LuokkaTaulukko<T> vasen, LuokkaTaulukko<T> oikea)
        {
            return vasen.Equals(oikea);
        }
        public static bool operator !=(LuokkaTaulukko<T> vasen, LuokkaTaulukko<T> oikea)
        {
            return !vasen.Equals(oikea);
        }

    }
}
