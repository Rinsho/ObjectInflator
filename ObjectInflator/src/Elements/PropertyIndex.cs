
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

internal class PropertyIndex : Property
{
    public IIterator Iterator { get; protected set; }

    public PropertyIndex(PropertyInfo property, IIterator iterator, IElement propertyObj)
        : base (property, propertyObj)
    {
        Iterator = iterator;
    }

    //IElement interface
    public override void Accept(IVisitor visitor) =>
        visitor.Visit(this);
}
