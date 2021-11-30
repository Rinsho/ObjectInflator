using NUnit.Framework;
using System;
using System.Collections.Generic;

//Field usage and assignment warnings are irrelevant.
#pragma warning disable CS0169
#pragma warning disable CS0649

//General note: I just realized if a type is an interface we can't construct it.
//    It's effectively [NoInit] by default. Force raw cast?

namespace ExpressionGen.Tests
{
    [TestFixture]
    public class ConstructionVisitorTests
    {
        class ObjectAsData
        {
            public int x;

            public ObjectAsData(int x) =>
                this.x = x;

            public static implicit operator ObjectAsData(int x) =>
                new ObjectAsData(x);
        }

        [Test]
        public void OnObjectAsData_CovertDataToObject()
        {
            //Arrange
            Element dataElement = ElementGenerator.CreateType<ObjectAsData>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());

            //Act
            dataElement.Accept(visitor);
            Func<int, ObjectAsData> func = visitor.GetResult<int, ObjectAsData>();
            ObjectAsData obj = func(10);

            //Assert
            Assert.That(obj.x, Is.EqualTo(10));
        }

        class ObjectWithField
        {
            [DataTarget(DataId = "x")]
            public int x;
        }

        [Test]
        public void OnFieldData_FillObjectWithFieldData()
        {
            //Arrange
            Element objectElement = ElementGenerator.CreateType<ObjectWithField>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("x", 5);

            //Act
            objectElement.Accept(visitor);
            Func<Dictionary<string, object>, ObjectWithField> func = 
                visitor.GetResult<Dictionary<string, object>, ObjectWithField>();
            ObjectWithField obj = func(data);

            //Assert
            Assert.That(obj.x, Is.EqualTo(5));
        }

        class ObjectWithProperty
        {
            [DataTarget(DataId = "y")]
            public string Y { get; private set; }

        }

        [Test]
        public void OnPropertyData_FillObjectWithPropertyData()
        {
            //Arrange
            Element objectElement = ElementGenerator.CreateType<ObjectWithProperty>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("y", "test");

            //Act
            objectElement.Accept(visitor);
            Func<Dictionary<string, object>, ObjectWithProperty> func =
                visitor.GetResult<Dictionary<string, object>, ObjectWithProperty>();
            ObjectWithProperty obj = func(data);

            //Assert
            Assert.That(string.Equals(obj.Y, "test", StringComparison.OrdinalIgnoreCase));
        }

        class ObjectWithMethod
        {
            public double X { get; private set; }

            public void SetX([DataTarget(DataId = "x")] double x) => X = x;
        }

        [Test]
        public void OnMethodData_CallMethodWithParameterData()
        {
            //Arrange
            Element objectElement = ElementGenerator.CreateType<ObjectWithMethod>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("x", 3.14159);

            //Act
            objectElement.Accept(visitor);
            Func<Dictionary<string, object>, ObjectWithMethod> func =
                visitor.GetResult<Dictionary<string, object>, ObjectWithMethod>();
            ObjectWithMethod obj = func(data);

            //Assert
            Assert.That(obj.X, Is.EqualTo(3.14159));
        }

        class ObjectWithConstructor
        {
            public string Value;
            public int X;

            public ObjectWithConstructor(
                [DataTarget(DataId = "c1")] string val,
                [DataTarget(DataId = "c2")] int x
            )
            {
                Value = val;
                X = x;
            }
        }

        [Test]
        public void OnConstructorData_CreateObjectWithParameterData()
        {
            //Arrange
            Element objectElement = ElementGenerator.CreateType<ObjectWithConstructor>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("c1", "testString");
            data.Add("c2", 12);

            //Act
            objectElement.Accept(visitor);
            Func<Dictionary<string, object>, ObjectWithConstructor> func =
                visitor.GetResult<Dictionary<string, object>, ObjectWithConstructor>();
            ObjectWithConstructor obj = func(data);

            //Assert
            Assert.That(string.Equals(obj.Value, "testString", StringComparison.OrdinalIgnoreCase));
            Assert.That(obj.X, Is.EqualTo(12));
        }

        class ObjectWithPropertyIndex
        {
            public int[] list = new int[] { 0, 1, 2, 3, 4, 5 };

            [DataTarget(DataId = "index")]
            public int this[int x]
            {
                get => list[x];
                set => list[x] = value;
            }
        }

        [Test]
        public void OnPropertyIndexData_FillObjectWithPropertyIndexData()
        {
            //Arrange
            Element objectElement = ElementGenerator.CreateType<ObjectWithPropertyIndex>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("index", new int[] { 5, 4, 3 });

            //Act
            objectElement.Accept(visitor);
            Func<Dictionary<string, object>, ObjectWithPropertyIndex> func =
                visitor.GetResult<Dictionary<string, object>, ObjectWithPropertyIndex>();
            ObjectWithPropertyIndex obj = func(data);

            //Assert
            Assert.That(obj.list, Is.EquivalentTo(new int[] { 5, 4, 3, 3, 4, 5 }));
        }

        class ObjectWithArray
        {
            [DataTarget(DataId = "nums")]
            public int[] numbers;
        }

        [Test]
        public void OnFieldArrayData_FillObjectWithArrayData()
        {
            //Arrange
            int[] dataArray = new int[] { 1, 2, 3, 4, 5 };
            Element objectElement = ElementGenerator.CreateType<ObjectWithArray>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("nums", dataArray);

            //Act
            objectElement.Accept(visitor);
            Func<Dictionary<string, object>, ObjectWithArray> func =
                visitor.GetResult<Dictionary<string, object>, ObjectWithArray>();
            ObjectWithArray obj = func(data); //this hangs

            //Assert
            Assert.That(obj.numbers, Is.EquivalentTo(dataArray));
        }

        class ObjectWithNestedObject
        {
            [DataTarget(DataId = "rawObj")]
            public ObjectAsData someData;
            [DataTarget]
            public ObjectWithProperty someOtherData;
        }

        [Test]
        public void OnNestedTypeData_FillObjectWithObjectData()
        {
            //Arrange
            Element objectElement = ElementGenerator.CreateType<ObjectWithNestedObject>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("rawObj", 1337);
            data.Add("y", "stringTest");

            //Act
            objectElement.Accept(visitor);
            Func<Dictionary<string, object>, ObjectWithNestedObject> func =
                visitor.GetResult<Dictionary<string, object>, ObjectWithNestedObject>();
            ObjectWithNestedObject obj = func(data);

            //Assert
            Assert.That(obj.someData.x, Is.EqualTo(1337));
            Assert.That(string.Equals(obj.someOtherData.Y, "stringTest", StringComparison.OrdinalIgnoreCase));
        }

        class Address
        {
            [DataTarget(DataId = "zip")]
            public int ZipCode { get; set; }
            [DataTarget(DataId = "addr")]
            public string StreetAddress { get; set; }
            [DataTarget(DataId = "state")]
            public string State { get; set; }

            public override bool Equals(object obj)
            {
                Address obj2 = (Address)obj;
                return (this.ZipCode == obj2.ZipCode &&
                    string.Equals(this.StreetAddress, obj2.StreetAddress) &&
                    string.Equals(this.State, obj2.State)
                );
            }
        }
        class Person
        {
            [DataTarget(DataId = "fn")]
            public string FirstName { get; set; }
            [DataTarget(DataId = "ln")]
            public string LastName { get; set; }
            [DataTarget(DataId = "addr*")]
            public Address[] Addresses { get; set; }

            public override bool Equals(object obj)
            {
                Person obj2 = (Person)obj;
                if (!string.Equals(this.FirstName, obj2.FirstName))
                    return false;
                if (!string.Equals(this.LastName, obj2.LastName))
                    return false;
                for (int i = 0; i < Addresses.Length; i++)
                    if (!this.Addresses[i].Equals(obj2.Addresses[i]))
                        return false;
                return true;
            }
        }

        [Test]
        public void OnComplexData_ConstructProperObject()
        {
            //Arrange
            Element objElement = ElementGenerator.CreateType<Person>();
            ConstructionVisitor visitor = new ConstructionVisitor(new DataContext(), new DataConverter());
            Dictionary<string, object> data = new Dictionary<string, object>() {
                ["fn"] = "John",
                ["ln"] = "Doe",
                ["addr*"] = new [] {
                    new Dictionary<string, object>() {
                        ["zip"] = 11111,
                        ["state"] = "CA",
                        ["addr"] = "000 Test blvd"
                    },
                    new Dictionary<string, object>() {
                        ["zip"] = 22222,
                        ["state"] = "WA",
                        ["addr"] = "111 Test st apt 1"
                    }
                }
            };

            //Act
            objElement.Accept(visitor);
            Func<Dictionary<string, object>, Person> func = visitor.GetResult<Dictionary<string, object>, Person>();
            Person obj = func(data);

            //Assert
            Person test = new Person();
            test.FirstName = "John";
            test.LastName = "Doe";
            test.Addresses = new [] {
                new Address() { ZipCode = 11111, State = "CA", StreetAddress = "000 Test blvd" },
                new Address() { ZipCode = 22222, State = "WA", StreetAddress = "111 Test st apt 1"}
            };
            Assert.That(obj.Equals(test));
        }
    }
}