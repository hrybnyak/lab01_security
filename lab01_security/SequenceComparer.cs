using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace lab01_security
{
    public class SequenceComparer : IEqualityComparer<List<string>>
    {
        public bool Equals(List<string> x, List<string> y)
        {
            return Enumerable.SequenceEqual(x, y);
        }

        public int GetHashCode([DisallowNull] List<string> obj)
        {
            return obj.GetHashCode();
        }
    }
}
