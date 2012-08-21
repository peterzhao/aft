using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Aft;

namespace SalsaImporterTests.Aft
{
    [TestFixture]
    public class CustomFieldValuesTests
    {
        private SupporterCustomField field1;
        private SupporterCustomField field2;
        private List<SupporterCustomField> fields;
        private SupporterCustomFieldValue value1;
        private Collection<SupporterCustomFieldValue> values;
        private CustomFieldValues fieldValues;

        [SetUp]
        public void SetUp()
        {
             field1 = new SupporterCustomField { Id = 1, Name = "field1", Type = "string" };
             field2 = new SupporterCustomField { Id = 2, Name = "field2", Type = "string" };
             fields = new List<SupporterCustomField> { field1, field2 };
             value1 = new SupporterCustomFieldValue { Id = 101, SupporterCustomField_Id = field1.Id, Value = "feild1Value" };
             values = new Collection<SupporterCustomFieldValue> { value1 };

             fieldValues = new CustomFieldValues(fields, values);
        }

        [Test]
        public void ShouldGetCustomFieldValue()
        {
            Assert.AreEqual(value1.Value, fieldValues[field1.Name]);
            Assert.IsNull(fieldValues[field2.Name]);
        }

        [Test]
        public void ShouldGetNames()
        {
            fieldValues[field2.Name] = "newfield2value";
            Assert.AreEqual(field1.Name, fieldValues.Names.First());
            Assert.AreEqual(field2.Name, fieldValues.Names.Last());
            Assert.AreEqual(2, fieldValues.Names.Count());
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void ShouldGetErrorWhenGivenInvalidFieldName()
        {
            var value = fieldValues["somethingNotInFieldDefination"];
        }

        [Test]
        public void ShouldSetCustomFieldValue()
        {
            fieldValues[field2.Name] = "newfield2value";
            Assert.AreEqual("newfield2value",fieldValues[field2.Name]);
        }

        [Test]
        public void ShouldUpdateCustomFieldValue()
        {
            fieldValues[field1.Name] = "newfield1value";
            Assert.AreEqual("newfield1value", fieldValues[field1.Name]);
        }

        [Test]
        public void ShouldOverrideToString()
        {
            Assert.AreEqual("field1:feild1Value", fieldValues.ToString());

            fieldValues[field2.Name] = "field2value";
            Assert.AreEqual("field1:feild1Value field2:field2value", fieldValues.ToString());
        }

        [Test]
        public void ShouldBeEqualInValues()
        {
            var value2 = new SupporterCustomFieldValue { Id = 101, SupporterCustomField_Id = field1.Id, Value = "feild1Value" };
            var values2 = new Collection<SupporterCustomFieldValue> { value2 };

            var fieldValues2 = new CustomFieldValues(fields, values2);

            Assert.AreEqual(fieldValues, fieldValues2);
        }

        [Test]
        public void ShouldBeEqualInValuesIgnoreEmptyAndNull()
        {
            var value2 = new SupporterCustomFieldValue { Id = 101, SupporterCustomField_Id = field1.Id, Value = " " };
            var values2 = new Collection<SupporterCustomFieldValue> { value2 };

            var fieldValues2 = new CustomFieldValues(fields, values2);

            fieldValues[field1.Name] = null;

            Assert.AreEqual(fieldValues, fieldValues2);
        }

        [Test]
        public void ShouldNotBeEqualInValues()
        {
            var value2 = new SupporterCustomFieldValue { Id = 101, SupporterCustomField_Id = field1.Id, Value = "feild2Value" };
            var values2 = new Collection<SupporterCustomFieldValue> { value2 };

            var fieldValues2 = new CustomFieldValues(fields, values2);

            Assert.AreNotEqual(fieldValues, fieldValues2);
        }


       
    }
}
