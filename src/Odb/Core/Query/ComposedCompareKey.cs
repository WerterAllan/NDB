using System.Text;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Query
{
    /// <summary>
    ///   A composed key : an object that contains various values used for indexing query result 
    ///   <p>This is an implementation that allows compare keys to contain more than one single value to be compared</p>
    /// </summary>
    internal sealed class ComposedCompareKey : CompareKey
    {
        private readonly IOdbComparable[] _keys;

        public ComposedCompareKey(IOdbComparable[] keys)
        {
            _keys = keys;
        }

        public override int CompareTo(object o)
        {
            if (o == null || o.GetType() != typeof (ComposedCompareKey))
                return -1;
            var ckey = (ComposedCompareKey) o;

            for (var i = 0; i < _keys.Length; i++)
            {
                var result = _keys[i].CompareTo(ckey._keys[i]);
                if (result != 0)
                    return result;
            }

            return 0;
        }

        public override string ToString()
        {
            if (_keys == null)
                return "no keys defined";

            var buffer = new StringBuilder();
            for (var i = 0; i < _keys.Length; i++)
            {
                if (i != 0)
                    buffer.Append("|");

                buffer.Append(_keys[i]);
            }

            return buffer.ToString();
        }
    }
}
