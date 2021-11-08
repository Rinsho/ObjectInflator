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
            [ParserData(DataID = "X")]
            public int x;

            [ParserData(DataID = "Y")]
            public string Y {get; set;}

            private string _z = string.Empty;
            [ParserData]
            public string Z { set => this._z = value; }
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            
            Assert.Pass();
        }
    }
}