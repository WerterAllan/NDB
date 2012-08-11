using System;
using System.Globalization;

namespace NDatabase.Odb.Impl.Core.Query.Values
{
    public static class ValuesUtil
    {
        public static Decimal Convert(Decimal number)
        {
            return System.Convert.ToDecimal(number.ToString(CultureInfo.InvariantCulture));
        }
    }
}
