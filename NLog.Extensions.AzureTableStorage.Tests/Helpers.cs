using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Extensions.AzureTableStorage.Tests
{
    public static class Helpers
    {
        public static string ExceptBlanks(this string str)
        {
            var sb = new StringBuilder(str.Length);
            foreach (var c in str.Where(c => !char.IsWhiteSpace(c)))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
