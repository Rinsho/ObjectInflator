using NUnit.Framework;
using System;
using System.Linq.Expressions;
using ExpressionGen;
using System.Collections.Generic;


//General note: I just realized if a type is an interface we can't construct it.
//    It's effectively [NoInit] by default.

//Generic Construction/Context still feels weird and brittle. Maybe wrap all incoming
//data in a Dict[""] = param to force a Dict<string, object>?

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
    }
}