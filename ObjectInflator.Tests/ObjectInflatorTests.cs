using NUnit.Framework;
using System;
using System.Linq.Expressions;
using ExpressionGen;
using System.Collections.Generic;

namespace ExpressionGen.Tests
{
    [TestFixture]
    public class ElementGeneratorTests
    {
        class ObjectWithNoSubTargets
        {
            public int X { get; set; }

            public ObjectWithNoSubTargets(int x) =>
                X = x;

            public static implicit operator ObjectWithNoSubTargets(int x) =>
                new ObjectWithNoSubTargets(x);
        }

        [Test]
        public void OnObjectWithNoSubTargets_CreateData()
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
        public void OnFieldTarget_CreateField()
        {
            //Arrange
            Type testType = typeof(ObjectWithValidField);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Constructor|Field|Data";
            Assert.That(
                string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase),
                result
            );
        }

        class ObjectWithValidProperty
        {
            [DataTarget(DataId = "X")]
            public int X { get; private set; }
        }

        [Test]
        public void OnPropertyTarget_CreateProperty()
        {
            //Arrange
            Type testType = typeof(ObjectWithValidProperty);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Constructor|Property|Data";
            Assert.That(
                string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase),
                result
            );
        }

        class ObjectWithValidMethod
        {
            private int _x;
            public void X([DataTarget(DataId = "x")] int x) => _x = x;
        }

        [Test]
        public void OnMethodTarget_CreateMethod()
        {
            //Arrange
            Type testType = typeof(ObjectWithValidMethod);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Constructor|Method|Data";
            Assert.That(
                string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase),
                result
            );
        }

        class ObjectWithNonDefaultConstructor
        {
            public ObjectWithNonDefaultConstructor(
                [DataTarget(DataId = "X")] int x,
                [DataTarget(DataId = "Y")] ObjectWithValidField y
            ) {}
        }
        
        [Test]
        public void OnNonDefaultConstructorTarget_CreateParameters()
        {
            //Arrange
            Type testType = typeof(ObjectWithNonDefaultConstructor);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Constructor|Data|Object|Constructor|Field|Data";
            Assert.That(
                string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase), 
                result
            );
        }

        class ObjectWithPropertyIndex
        {
            private Dictionary<int, int> _data;

            [DataTarget(DataId = "index")]
            public int this[int x]
            {
                get => _data[x];
                set => _data[x] = value;
            }
        }

        [Test]
        public void OnPropertyIndexTarget_CreatePropertyIndex()
        {
            //Arrange
            Type testType = typeof(ObjectWithPropertyIndex);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Constructor|PropertyIndex|Data";
            Assert.That(
                string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase),
                result
            );
        }

        class ObjectWithArrayMember
        {
            [DataTarget(DataId = "array")]
            public int[] X { get; private set; }
        }

        [Test]
        public void OnArrayTypeMemberTarget_CreateArray()
        {
            //Arrange
            Type testType = typeof(ObjectWithArrayMember);
            ElementToStringVisitor visitor = new ElementToStringVisitor();

            //Act
            IElement objectElement = ElementGenerator.CreateType(testType);
            objectElement.Accept(visitor);
            string result = visitor.Result;

            //Assert
            string expectedResult = "|Object|Constructor|Property|Array|Data";
            Assert.That(
                string.Equals(result, expectedResult, StringComparison.OrdinalIgnoreCase),
                result
            );
        }

        class ObjectWithMultidimensionalArray
        {
            [DataTarget(DataId = "a")]
            private int[,] _x;
        }

        [Test]
        public void OnMultidimensionalArrayMember_TypeNotSupportedException()
        {
            //Arrange
            Type testType = typeof(ObjectWithMultidimensionalArray);

            //Act and Assert
            Assert.Throws<TypeNotSupportedException>(
                () => ElementGenerator.CreateType(testType)
            );
        }

        class ObjectWithJaggedArray
        {
            [DataTarget(DataId = "b")]
            private int[][] _x;
        }

        [Test]
        public void OnJaggedArrayMember_TypeNotSupportedException()
        {
            //Arrange
            Type testType = typeof(ObjectWithJaggedArray);

            //Act and Assert
            Assert.Throws<TypeNotSupportedException>(
                () => ElementGenerator.CreateType(testType)
            );
        }

        class ObjectWithNoValidConstructor
        {
            [DataTarget(DataId = "x")]
            private int _x;

            public ObjectWithNoValidConstructor(int x) {}
        }

        [Test]
        public void OnObjectWithNoValidConstructor_TypeNotSupportedException()
        {
            //Arrange
            Type testType = typeof(ObjectWithNoValidConstructor);

            //Act and Assert
            Assert.Throws<TypeNotSupportedException>(
                () => ElementGenerator.CreateType(testType)
            );
        }
    }

    [TestFixture]
    public class ConstructionVisitorTests
    {

    }

    //Exercise this through ConstructionVisitorTests?
    public class DataContextTests
    {

    }
}