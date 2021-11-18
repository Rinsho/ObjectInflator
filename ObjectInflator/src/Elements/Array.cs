
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

internal class Array : IElement
{
    public Type Type { get; protected set; }
    public IElement ArrayObject { get; protected set; } 
    public IIterator Iterator { get; protected set; }

    public Array(Type arrayType, NumericIterator iterator, IElement arrayObj)
    {
        ArrayObject = arrayObj;
        Type = arrayType;
        Iterator = iterator;
        if (arrayType.IsDefined(typeof(DataTargetAttribute)))
            DataId = arrayType.GetCustomAttribute<DataTargetAttribute>().DataId;
    }

    //IElement interface
    public string DataId { get; protected set; }
    public IEnumerable<IElement> Children { get => new[] { ArrayObject }; }
    public void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
