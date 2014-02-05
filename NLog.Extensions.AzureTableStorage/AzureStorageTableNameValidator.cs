using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLog.Extensions.AzureTableStorage
{
    //validation rules descriped in: http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
    public class AzureStorageTableNameValidator
    {
        private readonly string _tableName;
        private const string RegularExpression = @"^[A-Za-z][A-Za-z0-9]{2,62}$";
        private readonly List<string> _reservedWords;

        public AzureStorageTableNameValidator(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new NullReferenceException(tableName);
            }
            _tableName = tableName;
            _reservedWords = new List<string> { "tables" };
        }

        public bool IsValid()
        {
            return !_reservedWords.Contains(_tableName) 
                && Regex.IsMatch(_tableName, RegularExpression);
        }
    }
}
