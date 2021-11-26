
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

internal class Array : Element
{
    public Type Type { get; protected set; }
    public Element ArrayObject { get; protected set; } 
    public IIterator Iterator { get; protected set; }

    public Array(Type arrayType, NumericIterator iterator, Element arrayObj)
    {
        ArrayObject = arrayObj;
        Type = arrayType;
        Iterator = iterator;
        if (arrayType.IsDefined(typeof(DataTargetAttribute)))
            DataId = arrayType.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public override IEnumerable<Element> Children { get => new[] { ArrayObject }; }
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
