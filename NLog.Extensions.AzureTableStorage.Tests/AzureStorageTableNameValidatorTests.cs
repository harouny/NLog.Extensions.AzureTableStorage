using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NLog.Extensions.AzureTableStorage.Tests
{
    public class AzureStorageTableNameValidatorTests
    {
        [Fact]
        public void IsValidReturnFalseIfTableNameIsReservedWord()
        {
            var validator = new AzureStorageTableNameValidator("tables"); //reserved (invalid)
            Assert.False(validator.IsValid());
        }


        [Fact]
        public void IsValidReturnFalseIfTableNameVaiolatesRules_number()
        {
            var validator = new AzureStorageTableNameValidator("5");//invalid
            Assert.False(validator.IsValid());
        }


        [Fact]
        public void IsValidReturnFalseIfTableNameVaiolatesRules_startsWithNumber()
        {
            var validator = new AzureStorageTableNameValidator("5products");//invalid
            Assert.False(validator.IsValid());
        }


        [Fact]
        public void IsValidReturnFalseIfTableNameVaiolatesRules_containSpaces()
        {
            var validator = new AzureStorageTableNameValidator("products and customers");//invalid
            Assert.False(validator.IsValid());
        }


        [Fact]
        public void IsValidReturnTrueIfTableNameIsValid()
        {
            var validator = new AzureStorageTableNameValidator("myTable");//valid name
            Assert.True(validator.IsValid());
        }

    }
}
