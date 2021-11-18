using NUnit.Framework;
using System;
using System.Linq.Expressions;
using ExpressionGen;

namespace ExpressionGen.Tests
{
    [TestFixture]
    public class Tests
    {
        class Test
        {
            [DataTarget(DataId = "X")]
            public int x;

            [DataTarget(DataId = "Y")]
            public string Y {get; set;}

            private string _z = string.Empty;
            [DataTarget]
            public string Z { set => this._z = value; }
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            
            Assert.Fail();
        }
    }
}