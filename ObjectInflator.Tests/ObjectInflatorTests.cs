using NUnit.Framework;
using System;
using System.Linq.Expressions;
using ExpressionGen;

namespace ExpressionGen.Tests
{
    [TestFixture]
    public class Tests
    {
        class NoTargets
        {
            public string x;
        }

        [Test]
        public void ElementGenerator_OnNoTargets_CreateData()
        {
            //Arrange
            Type testType = typeof(NoTargets);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement dataElement = ElementGenerator.CreateType(testType);
            dataElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Data";
            Assert.That(string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase));
        }

        [DataTarget(DataId = "X")]
        class ObjectWithNoSubTargets
        {
            public int X { get; set; }

            public ObjectWithNoSubTargets(int x) =>
                X = x;

            public static implicit operator ObjectWithNoSubTargets(int x) =>
                new ObjectWithNoSubTargets(x);
        }

        [Test]
        public void ElementGenerator_OnObjectWithNoSubTargets_CreateData()
        {
            //Arrange
            Type testType = typeof(ObjectWithNoSubTargets);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement dataElement = ElementGenerator.CreateType(testType);           
            dataElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Data";
            Assert.That(string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase));
        }

        class ObjectWithValidField
        {
            [DataTarget(DataId = "X")]
            public int X;
        }

        [Test]
        public void ElementGenerator_OnFieldTarget_CreateObject()
        {
            //Arrange
            Type testType = typeof(ObjectWithValidField);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Field|Data";
            Assert.That(string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase));
        }

        class ObjectWithValidProperty
        {
            [DataTarget(DataId = "X")]
            public int X { get; private set; }
        }

        [Test]
        public void ElementGenerator_OnPropertyTarget_CreateObject()
        {
            //Arrange
            Type testType = typeof(ObjectWithValidProperty);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Property|Data";
            Assert.That(string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase));
        }

        class ObjectWithValidMethod
        {
            private int _x;
            public void X([DataTarget(DataId = "x")] int x) => _x = x;
        }

        [Test]
        public void ElementGenerator_OnMethodTarget_CreateObject()
        {
            //Arrange
            Type testType = typeof(ObjectWithValidMethod);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Method|Data";
            Assert.That(string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase));
        }
    }
}