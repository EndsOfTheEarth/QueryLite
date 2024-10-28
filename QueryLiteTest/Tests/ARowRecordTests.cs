using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryLite;
using System.Collections.Generic;

namespace QueryLiteTest.Tests {

    [TestClass]
    public sealed class ARowRecordTests {

        [TestMethod]
        public void TestRowRecordSuccessfulValidation() {

            List<string> messages = [];

            RowValidator.ValidateType(typeof(ParentRow), messages);
            Assert.AreEqual(0, messages.Count);

            RowValidator.ValidateType(typeof(Child), messages);
            Assert.AreEqual(0, messages.Count);
        }

        [TestMethod]
        public void TestRowRecordInvalidValidation() {

            RowValidator.RowValidationResult result = RowValidator.ValidateRowsInAssembly(typeof(ParentRow).Assembly);

            Assert.IsTrue(result.HasMessages);
            Assert.AreEqual(1, result.Messages.Count);

            Assert.IsTrue(result.TypesValidated.Contains(typeof(ParentRow)));
            Assert.IsTrue(result.TypesValidated.Contains(typeof(Child)));
            Assert.IsTrue(result.TypesValidated.Contains(typeof(ChildWithInvalidProperty)));

            List<string> messages = [];

            RowValidator.ValidateType(typeof(ChildWithInvalidProperty), messages);
            Assert.AreEqual(1, messages.Count);
        }
    }

    internal record ParentRow : ARowRecord {

        public int Id { get; set; }
    }

    internal record Child : ParentRow {

        public string GivenName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
    }


    internal record ChildWithInvalidProperty : ParentRow {

        public string Name { get; set; } = string.Empty;
        public List<string> List { get; set; } = [];
    }
}